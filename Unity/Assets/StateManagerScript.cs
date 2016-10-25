using UnityEngine;
using System.Collections;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class StateManagerScript : MonoBehaviour {

    [SerializeField]
    GameObject LogoGo;

    [SerializeField]
    GameObject MenuGo;

    [SerializeField]
    GameObject InGameGo;

    [SerializeField]
    GameObject EndGameGo;

    [SerializeField]
    Button PlayGameButton;

    [SerializeField]
    Button RestartGameButton;

    [SerializeField]
    Button QuitGameButton;

    [SerializeField]
    float startLogoDelai;

    [SerializeField]
    float inGameDelai;

    // Use this for initialization
    void Start()
    {
        // NEW GAMESTATESTREAM WITH VIEW BINDINGS
        var gameStateStream = Observable.Return("start")
            .Concat(Observable.Return("logo"))
            .Concat(Observable.Return("menu").Delay(TimeSpan.FromSeconds(startLogoDelai)))
            .Concat(PlayGameButton.OnClickAsObservable().Take(1).Select(_ => "inGame"))
            .Concat(Observable.Return("endGame").Delay(TimeSpan.FromSeconds(inGameDelai)))
            .Concat(RestartGameButton.OnClickAsObservable().Take(1).Select(_ => "restartGame"))
            //.Concat(QuitGameButton.OnClickAsObservable().Take(1).Select(_ => "restartGame"))
            .Repeat().Share();

        // Specialized gamestate streams
        var startGameStream = gameStateStream.Where(state => state == "start");
        var logoGameStream = gameStateStream.Where(state => state == "logo");
        var menuGameStream = gameStateStream.Where(state => state == "menu");
        var inGameGameStream = gameStateStream.Where(state => state == "inGame");
        var endGameGameStream = gameStateStream.Where(state => state == "endGame");
        var restartGameGameStream = gameStateStream.Where(state => state == "restartGame");

        // Root game objects activation streams
        var logoIsActiveStream = logoGameStream.Select(_ => true)
            .Merge(menuGameStream.Select(_ => false));
        var menuIsActiveStream = menuGameStream.Select(_ => true)
            .Merge(inGameGameStream.Select(_ => false));
        var inGameIsActiveStream = inGameGameStream.Select(_ => true)
            .Merge(endGameGameStream.Select(_ => false));
        var endGameIsActiveStream = endGameGameStream.Select(_ => true)
            .Merge(startGameStream.Select(_ => false));
        var restartGameIsActiveStream = restartGameGameStream.Select(_ => true)
            .Merge(inGameGameStream.Select(_ => false));

        // Root gameobjects activation bindings
        logoIsActiveStream.Subscribe(LogoGo.SetActive);
        menuIsActiveStream.Subscribe(MenuGo.SetActive);
        inGameIsActiveStream.Subscribe(InGameGo.SetActive);
        endGameIsActiveStream.Subscribe(EndGameGo.SetActive);

        // internal custom game State Stream
        var inGameGameStateStream = inGameGameStream.SelectMany(_ => Observable.EveryFixedUpdate())
            .TakeUntil(endGameGameStream)
            .Scan(new InGameCustomGameState(), (gameState, ticks) =>
            GetNextState(gameState)
        ).Repeat();

        // Side effect of the inGame GameState change (update the view)
        inGameGameStateStream.Subscribe(ApplyGameState);
    }

    // Simple inGame InGameCustomGameState
    public struct InGameCustomGameState
    {
        public float PosX;
    }

    public InGameCustomGameState GetNextState(InGameCustomGameState currentGameCustomState)
    {
        return new InGameCustomGameState { PosX = currentGameCustomState.PosX + 0.02f };
    }

    public void ApplyGameState(InGameCustomGameState gameCustomState)
    {
        
    }
}
