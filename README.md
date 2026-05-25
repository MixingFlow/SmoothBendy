# Smooth Bendy Mod - Chapter 3 Blocker Fix

## How to modify Assembly-CSharp.dll using dnSpy

1. Run `tools/dnSpy/dnSpy.exe` and open `Assembly-CSharp.dll`.
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
6. Click **File -> Save Module...** and save the DLL to `out/Assembly-CSharp.dll`.
