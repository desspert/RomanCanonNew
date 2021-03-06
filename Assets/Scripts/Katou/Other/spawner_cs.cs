﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spawner_cs : MonoBehaviour {
    //エネミーの種類
    [SerializeField, Tooltip("エネミーの種類")]
    private GameObject[] enemy = new GameObject[5];

    //敵を生成する時間
    [SerializeField, Tooltip("敵を生成する時間")]
    private float interval = 1.0f;

    //playerからリスポーンする場所までの距離
    [SerializeField, Tooltip("playerからリスポーンする場所までの距離")]
    private float radius = 100.0f;

    //最大でスポーンする位置数
    private int max_spawn = 200;

    //リスポーンする位置
    private Vector3[] spawn_pos;

    //リスポーンする際の角度
    private float[] angle;

    //ボスリスポーンするフラグ
    private bool boss_spawn;

    //プレイヤーする位置
    public Vector3 playerPos;

    private float[] inversion_Dir;

    public float inversion;

    [SerializeField]
    public GameObject score;


    Dictionary<int, int> enemy_type = new Dictionary<int, int>()
            {
                { 0, 40},           //ノーマル
                { 1, 25},           //スピード
                { 2, 20},           //ヘビー
                { 3, 15}            //ジャンプ
            };

    void Awake()
    {
        boss_spawn = false;
        spawn_pos = new Vector3[max_spawn];
        angle = new float[max_spawn];
        inversion_Dir = new float[max_spawn];
    }
    
    void Start()
    {
        for (int i = 0; i < spawn_pos.Length; i++)
        {
            float radian = i * Mathf.PI / 180.0f;

            float x1 = Mathf.Cos(radian) * radius + playerPos.x;
            float z1 = Mathf.Sin(radian) * radius + playerPos.z;

            spawn_pos[i] = new Vector3(x1, 0.0f, z1);

            angle[i] = 270 - i;

            inversion_Dir[i] = 180 + i;
        }

        StartCoroutine("Spawn", interval);
    }

    IEnumerator Spawn(float time)
    { 
        while (GameObject.Find("gamemanager").GetComponent<gameman>().end == false)
        {
            int type = probability.enemy_election(enemy_type);

            int count = Random.Range(0, spawn_pos.Length - 1);

            inversion = inversion_Dir[count];

            if (GameObject.Find("Timer").GetComponent<timer>().countTimer <= 30.0f && !boss_spawn)
            {
               GameObject obj = (GameObject)Instantiate(enemy[4], spawn_pos[count], Quaternion.Euler(0.0f, angle[count], 0.0f));
                obj.GetComponent<enemy_state>().score = score;
                boss_spawn = true;
            }
            else
            {
                GameObject obj = (GameObject)Instantiate(enemy[type], spawn_pos[count], Quaternion.Euler(0.0f, angle[count], 0.0f));
                obj.GetComponent<enemy_state>().score = score;
            }

            yield return new WaitForSeconds(time);
        }
    }
}
