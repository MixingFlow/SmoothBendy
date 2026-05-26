# Smooth Bendy Mod - with MixingFlow's fixes
This mod is made by jrgray93, I only want to make some fixes myself, as it takes a while to find issues alone.
Would be cool if the original mod got a GitHub Repo itself :D

## Installation
Just go to the Releases tab on the right and download the DLLs and replace the DLLs in your game's directory at  
`...\Bendy and the Ink Machine\Bendy and the Ink Machine_Data\Managed\`.

## How to modify Assembly-CSharp.dll yourself using dnSpy

1. Run `.../dnSpy/dnSpy.exe` and open `Assembly-CSharp.dll`.
2. Locate the `CH3BridgeBlocker` class.
3. Right-click inside `CH3BridgeBlocker` in the code viewer, select **Edit Class (C#)**, and add:
   ```csharp
   private bool m_IsPositionInitialized;
   ```
   Click **Compile**.
4. Right-click the class again, select **Edit Class (C#)**, and add this method:
   ```csharp
   private void EnsureInitialized()
   {
       if (this.m_IsPositionInitialized)
       {
           return;
       }
       this.m_IsPositionInitialized = true;
       this.m_GateOriginPosition = this.m_Gates[0].localPosition;
       this.m_GateDownPosition = this.m_GateOriginPosition;
       this.m_GateDownPosition.z = this.m_GateDownPosition.z - 7.75f;
   }
   ```
   Click **Compile**.
5. Edit the following methods:
   - **InitOnComplete()**: Right-click -> Edit Method, replace body with:
     ```csharp
     base.InitOnComplete();
     this.m_Blocker.SetActive(false);
     this.m_LightController.TurnOff();
     this.EnsureInitialized();
     ```
   - **Close()**: Add `this.EnsureInitialized();` to the top of the method body.
   - **ForceOpen()**: Add `this.EnsureInitialized();` to the top of the method body.
   - **Open()**: Add `this.EnsureInitialized();` to the top of the method body.
6. Click **File -> Save Module...** and save the DLL to `.../Bendy and the Ink Machine/Bendy and the Ink Machine_Data/Managed/Assembly-CSharp.dll`.

You can also do this with the dnSpy.Console.exe, but then you know what you're doing anyways.  
Also the above steps may not include all fixes in the future if I make more fixes, so just use the latest release if you have no idea.
