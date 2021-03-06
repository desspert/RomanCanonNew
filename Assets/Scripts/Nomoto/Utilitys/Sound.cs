﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 2016-10-04
/// @auther 野本
/// ネットから拾ったやつ
/// 処理が重い可能性あり
/// 様子を見てから、変更すべき点が分かれば変更します 
///
//Sound管理
public class Sound
{
    //SEチャンネル数
    const int SE_CHANNEL = 4;

    enum SoundType
    {
        BGM,
        SE
    }

    //シングルトン生成
    static Sound _instance = null;

    public static Sound GetInstance()
    {
        return _instance  ?? (_instance = new Sound());
    }

    //サウンド再生用のゲームオブジェクト
    private GameObject _object = null;
    //サウンドリソース


    AudioSource _sourceBgm = null; //BGM
    AudioSource _sourceSeDefault = null; //SE (複数回音を重なられる)
    AudioSource[] _sourceSeArray; //SE (指定した回数までしか音を重ねられない)

    Dictionary<string, _Data> _poolBgm = new Dictionary<string, _Data>();

    Dictionary<string, _Data> _poolSe = new Dictionary<string, _Data>();

    class _Data
    {
        public string Key;

        public string ResName;

        public AudioClip Clip;

        public _Data(string key,string res)
        {
            Key = key;
            ResName = "Sounds/" + res;
            Clip = Resources.Load(ResName) as AudioClip;
        }
    }

    public Sound()
    {
        _sourceSeArray = new AudioSource[SE_CHANNEL];
    }

    AudioSource _GetAudioSource(SoundType type,int channel = 1)
    {
        if (_object == null)
        {
            // GameObjectがなければ作る
            _object = new GameObject("Sound");
            // 破棄しないようにする
            GameObject.DontDestroyOnLoad(_object);
            // AudioSourceを作成
            _sourceBgm = _object.AddComponent<AudioSource>();
            _sourceSeDefault = _object.AddComponent<AudioSource>();
            for (int i = 0; i < SE_CHANNEL; i++)
            {
                _sourceSeArray[i] = _object.AddComponent<AudioSource>();
            }
        }

        if (type == SoundType.BGM)
        {
            // BGM
            return _sourceBgm;
        }
        else
        {
            // SE
            if (0 <= channel && channel < SE_CHANNEL)
            {
                // チャンネル指定
                return _sourceSeArray[channel];
            }
            else
            {
                // デフォルト
                return _sourceSeDefault;
            }
        }
    }

    // サウンドのロード
    // ※Resources/Soundsフォルダに配置すること
    public static void LoadBgm(string key, string resName)
    {
        GetInstance()._LoadBgm(key, resName);
    }
    public static void LoadSe(string key, string resName)
    {
        GetInstance()._LoadSe(key, resName);
    }
    void _LoadBgm(string key, string resName)
    {
        if (_poolBgm.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            _poolBgm.Remove(key);
        }
        _poolBgm.Add(key, new _Data(key, resName));
    }
    void _LoadSe(string key, string resName)
    {
        if (_poolSe.ContainsKey(key))
        {
            // すでに登録済みなのでいったん消す
            _poolSe.Remove(key);
        }
        _poolSe.Add(key, new _Data(key, resName));
    }

    /// BGMの再生
    /// ※事前にLoadBgmでロードしておくこと
    public static bool PlayBgm(string key)
    {
        return GetInstance()._PlayBgm(key);
    }
    bool _PlayBgm(string key)
    {
        if (_poolBgm.ContainsKey(key) == false)
        {
            // 対応するキーがない
            return false;
        }

        // いったん止める
        _StopBgm();

        // リソースの取得
        var _data = _poolBgm[key];

        // 再生
        var source = _GetAudioSource(SoundType.BGM);
        source.loop = true;
        source.clip = _data.Clip;
        source.Play();

        return true;
    }
    /// BGMの停止
    public static bool StopBgm()
    {
        return GetInstance()._StopBgm();
    }
    bool _StopBgm()
    {
        _GetAudioSource(SoundType.BGM).Stop();

        return true;
    }

    /// SEの再生
    /// ※事前にLoadSeでロードしておくこと
    public static bool PlaySe(string key, int channel = -1)
    {
        return GetInstance()._PlaySe(key, channel);
    }
    bool _PlaySe(string key, int channel = -1)
    {
        if (_poolSe.ContainsKey(key) == false)
        {
            // 対応するキーがない
            return false;
        }

        // リソースの取得
        var _data = _poolSe[key];

        if (0 <= channel && channel < SE_CHANNEL)
        {
            // チャンネル指定
            var source = _GetAudioSource(SoundType.SE, channel);
            source.clip = _data.Clip;
            source.Play();
        }
        else
        {
            // デフォルトで再生
            var source = _GetAudioSource(SoundType.SE);
            source.PlayOneShot(_data.Clip);
        }

        return true;
    }

}
