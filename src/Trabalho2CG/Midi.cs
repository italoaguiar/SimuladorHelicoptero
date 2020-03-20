using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Midi: IDisposable
{
    private int device = 0;

    [DllImport("winmm.dll")]
    static extern int midiOutOpen(out IntPtr hMidiOut, int iDevice,
                         int uiCallback, int uiInstance, int uiFlags);

    

    [DllImport("winmm.dll")]
    static extern int midiOutClose(IntPtr hMidiOut);

    [DllImport("winmm.dll")]
    static extern int midiOutShortMsg(IntPtr hMidiOut, uint iMsg);

    [DllImport("winmm.dll", EntryPoint = "midiInGetNumDevs")]
    private static extern int midiInGetNumDevs();

    [DllImport("winmm.dll", EntryPoint = "midiOutGetNumDevs")]
    private static extern int midiOutGetNumDevs();

    [DllImport("winmm.dll", EntryPoint = "midiInGetDevCapsW")]
    private static extern int midiInGetDevCapsW(int uDeviceID, ref MIDIINCAPSW pmic, int cbmic);

    [DllImport("winmm.dll", EntryPoint = "midiOutGetDevCapsW")]
    private static extern int midiOutGetDevCapsW(int uDeviceID, ref MIDIOUTCAPSW pmoc, int cbmoc);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MIDIINCAPSW
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public int dwSupport;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MIDIOUTCAPSW
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;
        public short wTechnology;
        public short wVoices;
        public short wNotes;
        public short wChannelMask;
        public int dwSupport;
    }

    IntPtr hMidiOut;

    public Midi()
    {
        // Most midiOutOpen arguments are not used here.
        int iResult = midiOutOpen(out hMidiOut, device, 0, 0, 0);

        if (iResult != 0)
            throw new Exception("midiOutOpen error number " + iResult);
    }

    ~Midi()
    {
        Dispose(false);
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected void Dispose(bool bDisposing)
    {
        if (hMidiOut != IntPtr.Zero)
        {
            midiOutClose(hMidiOut);
            hMidiOut = IntPtr.Zero;
        }
    }
    void Close()
    {
        Dispose();
    }
    public List<string> GetDevices()
    {
        List<string> r = new List<string>();
        for (int t = 0; t < midiOutGetNumDevs(); t++)
        {
            MIDIOUTCAPSW ocaps = new MIDIOUTCAPSW();
            midiOutGetDevCapsW(t, ref ocaps, Marshal.SizeOf(ocaps));
            r.Add(string.Format("{0}|{1}",ocaps.wMid, ocaps.szPname));
            
        }
        return r;
    }
    public void SetDevice(int device)
    {
        this.device = device;
    }
    public void Message(uint iStatus, uint iData1, uint iData2)
    {
        // Combine parameters into a single message.
        uint iMsg = iStatus | iData1 << 8 | iData2 << 16;
        int uiResult = midiOutShortMsg(hMidiOut, iMsg);
    }
    public void Instrument(uint iChannel, uint iInstrument)
    {
        Message(0xC0 | iChannel, iInstrument, 0);
    }
    public void NoteOn(uint iChannel, uint iNote, uint iVelocity)
    {
        //LogWriter.LogWrite(string.Format("NoteOn {0}{1}{2}",iChannel,iNote,iVelocity));
        Message(0x90 | iChannel, iNote, iVelocity);
    }
    public void NoteOff(uint iChannel, uint iNote)
    {
        NoteOn(iChannel, iNote, 0);
    }
    public void NoteOn(uint iChannel, string strNote, uint iVelocity)
    {
        if (strNote.ToUpper()[0] == 'R')    // Rest
            return;

        uint iNote = (uint)"C D EF G A B".IndexOf(strNote.ToUpper()[0]);
        int i = 2;     // assumed index of octave number in string
        
        if (strNote[1] == '#')
            iNote++;

        else if (strNote[1] == 'b')
            iNote--;

        else
            i = 1;    // index of octave number in string 

        iNote += (uint)(12 * Int32.Parse(strNote.Substring(i)));
        NoteOn(iChannel, iNote, iVelocity);
    }
    public void NoteOff(uint iChannel, string strNote)
    {
        NoteOn(iChannel, strNote, 0);
    }
}