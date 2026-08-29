using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBgm : MonoBehaviour
{
    public enum Mode {Play, Stop, KeepPrevious}

    [SerializeField] private Mode mode = Mode.Play;
    [SerializeField] private BgmPlaylist playlist;
    [SerializeField] private bool restartIfSame = false;

    private void Start()
    {
        SoundManager.EnsureExists();
        var sm = SoundManager.instance;
        if (sm == null) return;

        switch (mode)
        {
            case Mode.Play: sm.PlayBgm(playlist, restartIfSame); break;
            case Mode.Stop: sm.StopBgm(); break;
            case Mode.KeepPrevious: sm.KeepBgm(); break;
        }
        if(SoundManager.instance != null)
            SoundManager.instance.PlayBgm(playlist, restartIfSame);
    }
}
