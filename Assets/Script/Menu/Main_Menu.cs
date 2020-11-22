using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{

    //３つのPanelを格納する変数
    //インスペクターウィンドウからゲームオブジェクトを設定する
    [SerializeField] GameObject M_Menu;
    [SerializeField] GameObject Setting_Menu;
    [SerializeField] GameObject Description_Menu;
    // Start is called before the first frame update
    void Start()
    {
       
        BackToMainMenu();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ゲームシーンに遷移する
    public void GameStage(){
        SceneManager.LoadScene("O_FPS_1");
    }

    
    
    //ゲーム終了ボタンを押したら実行する
    public void EndGame() {
	    
		#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
         #endif
	    
    }

    //ゲーム設定のメニュー
    public void GameSetting(){
        M_Menu.SetActive(false);
        Setting_Menu.SetActive(true);
        
    }

    //ゲーム説明のメニュー
    public void GameDescription(){
        M_Menu.SetActive(false);
        Description_Menu.SetActive(true);
    }

    //メインメニューに戻る
    public void BackToMainMenu(){
        M_Menu.SetActive(true);
        Setting_Menu.SetActive(false);
        Description_Menu.SetActive(false);
    }
}
