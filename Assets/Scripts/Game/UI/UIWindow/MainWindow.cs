using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainWindow : UIWIndow
{
    [SerializeField]
    Button CloseBtn;
    public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel;
    public Image trackSelectImage, racerSelectImage;

    public struct WindowParam
    {
        int i;
    }

    public override void OnAwake()
    {
        buttonClickActionMap.Add(CloseBtn, QuitGame);
        //buttonClickActionMap.Add(_btn1, Btn1Click);
        EventDispatcher.AddListener(typeof(TestEventPram).Name, EventTestLog);
        EventDispatcher.AddListener(typeof(SpritePram).Name, ChangeImage);
        EventDispatcher.AddListener(typeof(TrackSpritePram).Name, trackImage);
    }

    public override void BeforeShow()
    {
       
    }

    protected override void InitWindow<T>(T? obj) where T : struct
    {
        var param = obj as WindowParam?;
        if (param == null)
            return;

    }

    public void StartGame()
    {
      
        ToCloseWindow();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }


    public void OpenRaceSetup()
    {
        raceSetupPanel.SetActive(true);
    }

    public void CloseRaceSetup()
    {
        raceSetupPanel.SetActive(false);
        //trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
    }

    public void OpenTrackSelect()
    {
        trackSelectPanel.SetActive(true);
        CloseRaceSetup();
    }
    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    public void OpenRacerSelect()
    {
        racerSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseRacerSelect()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }
    void EventTestLog(IBaseEventStruct arg)
    {
        if (arg is TestEventPram)
        {
            DebugCtrl.Log(((TestEventPram)arg).param1);
        }
    }

    void ChangeImage(IBaseEventStruct arg)
    {
        if (arg is SpritePram)
            racerSelectImage.sprite = ((SpritePram)arg).img;
    } 
    void trackImage(IBaseEventStruct arg)
    {
        if (arg is TrackSpritePram)
            trackSelectImage.sprite = ((TrackSpritePram)arg).img;
    }

    void Btn1Click()
    {
        EventDispatcher.TriggerEvent(new TestEventPram() { param1 = 11 });
    }
}
