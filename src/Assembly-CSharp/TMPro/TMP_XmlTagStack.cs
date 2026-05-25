namespace TMPro;

public struct TMP_XmlTagStack<T>(T[] tagStack)
{
	public T[] itemStack = tagStack;

	public int index = 0;

	public void Clear()
	{
		index = 0;
	}

	public void SetDefault(T item)
	{
		itemStack[0] = item;
		index = 1;
	}

	public void Add(T item)
	{
		if (index < itemStack.Length)
		{
			itemStack[index] = item;
			index++;
		}
	}

	public T Remove()
	{
		index--;
		if (index <= 0)
		{
			index = 0;
			return itemStack[0];
		}
		return itemStack[index - 1];
	}

	public T CurrentItem()
	{
		if (index > 0)
		{
			return itemStack[index - 1];
		}
		return itemStack[0];
	}

	public T PreviousItem()
	{
		if (index > 1)
		{
			return itemStack[index - 2];
		}
		return itemStack[0];
	}
}
