﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : SingletonMono<SceneChanger>
{
    //fadeさせるためのTexture
    public Texture2D _fadeTexture;

    //Alpha値
    private float _fadeAlpha = 0.0f;

    //fadeしているか
    private bool _isFading = false;

    static GameObject _instance = null;

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

       
        
    }

    public void OnGUI()
    {
        if (!this._isFading)
            return;

        //透明度を更新して黒テクスチャを描画
        GUI.color = new Color(0, 0, 0, this._fadeAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this._fadeTexture);
    }

    public void LoadLevel(string scene, float interval)
    {
        StartCoroutine(TransScene(scene, interval));
    }

    private IEnumerator TransScene(string scene, float interval)
    {
        //暗くなる処理
        _isFading = true;
        float _time = 0;
        while (_time <= interval)
        {
            _fadeAlpha = Mathf.Lerp(0f, 1f, _time / interval);
            _time += Time.deltaTime;
            yield return 0;
        }

        //シーン切替
        SceneManager.LoadScene(scene);

        //明るくなる処理
        _time = 0;
        while (_time <= interval)
        {
            _fadeAlpha = Mathf.Lerp(1f, 0f, _time / interval);
            _time += Time.deltaTime;
            yield return 0;
        }

        _isFading = false;
    }
}

