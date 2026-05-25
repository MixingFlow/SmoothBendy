using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using TMG.Data;
using UnityEngine;

public static class AsyncFileAPI
{
	private class State
	{
		private FileStream fStream;

		private byte[] writeArray;

		private byte[] readArray;

		private ManualResetEvent manualEvent;

		public FileStream FStream => fStream;

		public byte[] WriteArray => writeArray;

		public byte[] ReadArray => readArray;

		public ManualResetEvent ManualEvent => manualEvent;

		public State(FileStream fStream, byte[] writeArray, ManualResetEvent manualEvent)
		{
			this.fStream = fStream;
			this.writeArray = writeArray;
			this.manualEvent = manualEvent;
			readArray = new byte[writeArray.Length];
		}
	}

	public static Thread _thread;

	private static bool _started;

	private static string _fileName;

	private static GameData _data;

	private static ManualResetEvent manualEvent;

	public static event Action OnFileWriteErrorEvent;

	public static event Action OnFileWrittenEvent;

	public static void SaveData(GameData gameData, string fileName)
	{
		if (_started)
		{
			Debug.LogError((object)("SaveData call already in progress on file '" + _fileName + "'..."));
			return;
		}
		_fileName = fileName;
		_data = gameData;
		_thread = new Thread(ThreadedSave);
		_thread.Start();
	}

	private static void ThreadedSave()
	{
		_started = true;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, _data);
			array = memoryStream.ToArray();
		}
		manualEvent = new ManualResetEvent(initialState: false);
		FileStream fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, useAsync: true);
		fileStream.BeginWrite(array, 0, array.Length, EndWriteCallback, new State(fileStream, array, manualEvent));
		manualEvent.WaitOne();
		if (AsyncFileAPI.OnFileWrittenEvent != null)
		{
			AsyncFileAPI.OnFileWrittenEvent();
		}
		_started = false;
	}

	private static void EndWriteCallback(IAsyncResult asyncResult)
	{
		State state = (State)asyncResult.AsyncState;
		FileStream fStream = state.FStream;
		fStream.EndWrite(asyncResult);
		fStream.Position = 0L;
		asyncResult = fStream.BeginRead(state.ReadArray, 0, state.ReadArray.Length, EndReadCallback, state);
	}

	private static void EndReadCallback(IAsyncResult asyncResult)
	{
		State state = (State)asyncResult.AsyncState;
		int num = state.FStream.EndRead(asyncResult);
		int num2 = 0;
		while (num2 < num)
		{
			if (state.ReadArray[num2] != state.WriteArray[num2++])
			{
				Console.WriteLine("Error writing data.");
				Debug.LogError((object)"Error writing data.");
				state.FStream.Close();
				return;
			}
		}
		state.FStream.Close();
		state.ManualEvent.Set();
	}
}
