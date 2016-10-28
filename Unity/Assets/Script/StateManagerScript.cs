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
    Text endGameText;

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

    [SerializeField]
    GameObject mainCamera;

    bool iaIsDead = false;

    ReactiveProperty<bool> endGame = new ReactiveProperty<bool>();

    GameManagerScript gameManagerScript;

    public void SetGameManagerScript(GameManagerScript gmScript)
    {
        this.gameManagerScript = gmScript;
    }

    public void EndGame(bool isDead)
    {
        //Debug.Log("isdead= "+ isDead);
        iaIsDead = isDead;
        endGame.SetValueAndForceNotify(true);
    }

    // Use this for initialization
    void Start()
    {
        // NEW GAMESTATESTREAM WITH VIEW BINDINGS
        var gameStateStream = Observable.Return("start")
            .Concat(Observable.Return("logo"))
            .Concat(Observable.Return("menu").Delay(TimeSpan.FromSeconds(startLogoDelai)))
            .Concat(PlayGameButton.OnClickAsObservable().Take(1).Select(_ => "inGame"))
            .Concat(endGame.Where(val => val==true).Take(1).Select(_ => "endGame"))
            .Concat(RestartGameButton.OnClickAsObservable().Take(1).Select(_ => "restartGame"))
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


        var pauseStatusStream = inGameGameStream
            .Take(1)
            .SelectMany(_ =>
                Observable.EveryUpdate()
                .Where(__ => Input.GetKeyDown(KeyCode.P))
                .Scan(false, (pauseStatus, __) => !pauseStatus)
                .StartWith(false)
            )
            .TakeUntil(endGameGameStream);
            

        // Root gameobjects activation bindings
        logoIsActiveStream.Subscribe(LogoGo.SetActive);
        menuIsActiveStream.Subscribe(MenuGo.SetActive);
        inGameIsActiveStream.Subscribe(_ => {
            mainCamera.SetActive(false);
            InGameGo.SetActive(_);
        });
        endGameIsActiveStream.Subscribe(_ => {
            mainCamera.SetActive(true);
            endGameText.text = iaIsDead ? "IA LOSE" : "IA WIN";
            EndGameGo.SetActive(_);
            iaIsDead = false;
            endGame.SetValueAndForceNotify(false);
            gameManagerScript.InitializeGameState();
        });

        // internal custom game State Stream
        var inGameGameStateStream = inGameGameStream
            .SelectMany(_ => Observable.EveryFixedUpdate())
            .WithLatestFrom(pauseStatusStream, (_, pauseStatus) => pauseStatus)
            .Where(pauseStatus => !pauseStatus)
            .TakeUntil(endGameGameStream)
            //.Scan(gameManagerScript.ActualGameState, (gameState, ticks) => GetNextState(gameState))
            .Repeat();

        // Side effect of the inGame GameState change (update the view)
        inGameGameStateStream
            .SubscribeOn(Scheduler.MainThread).ObserveOn(Scheduler.MainThread).Subscribe(_ => {
                //GameState gs = gameManagerScript.ActualGameState;
                //gs.iaDirection = gameManagerScript.IaManagerScript.getNextIaDirection(gameManagerScript.ActualGameState);
                //gameManagerScript.ActualGameState = gs;
                //gameManagerScript.InitializeScripts();

                if (gameManagerScript.IAScript != null)
                {
                    gameManagerScript.ActualGameState.iaDirection = gameManagerScript.IAScript.GetNextDirectionIA();
                }
                else if(gameManagerScript.IAWithAStarScript != null)
                {
                    gameManagerScript.ActualGameState.iaDirection = gameManagerScript.IAWithAStarScript.GetNextDirectionIA();
                }
                GetNextState(gameManagerScript.ActualGameState);
                ApplyGameState();
            });

    }

    private void GetNextState(GameState gameState)
    {
        gameManagerScript.CollisionManagerScript.HandleBombCollision(gameState);
        gameState.timeSinceStart = Time.time * 1000;
    }

    private void ApplyGameState()
    {
        //A completer
        gameManagerScript.CollisionManagerScript.ApplyState(gameManagerScript.ActualGameState);


        //gameManagerScript.IaManagerScript.ApplyStateToIa(gameManagerScript.ActualGameState);
    }
}
