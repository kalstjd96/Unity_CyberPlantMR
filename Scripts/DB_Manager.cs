using System.Collections;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Collections.Generic;

public class DB_Manager : MonoBehaviour
{
    public Coroutine mainCor_DBManager;
    public static DB_Manager instance;
    SqliteConnection sqlConn;
    string dbPath;
    

    private void Awake()
    {
        if (instance == null) //싱글톤 
        {
            instance = this;
        }
    //#if UNITY_EDITOR
    //        dbPath = @"Data Source = " + Application.streamingAssetsPath + "/" + "AR_SEQ.db";
    //#elif WINDOWS_UWP
    //        ProjectManager.instance.Debugging("Database Copy 중...");
    //        Caching.ClearCache();
    //        mainCor_DBManager = StartCoroutine(CopyDatabase());
    //#endif
    }
    
    #region SqliteDB
    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AR_SEQ.db");
        dbPath = @"Data Source = " + path;
    }

    public DataTable Select(string query)
    {
        try
        {
            DataTable ResDT = new DataTable();
            SqliteDataAdapter Sda = new SqliteDataAdapter();
            sqlConn = new SqliteConnection(dbPath);
            sqlConn.Open();
            Sda.SelectCommand = new SqliteCommand(query, sqlConn);
            Sda.Fill(ResDT);
            //sqlConn.Close();
            ProjectManager.instance.Debugging("SQL 연결성공");
            return ResDT;
        }
        catch (Exception ex)
        {
            ProjectManager.instance.Debugging("SQL 연결실패 : " + ex);
            return null;
        }
    }

    public void SQL_Command(string ResultQuery)
    {
        //sqlConn.Open();
        SqliteTransaction tran = sqlConn.BeginTransaction(); ///트렌젝션 시작
        SqliteCommand cmd = new SqliteCommand();

        cmd.Transaction = tran;

        try
        {
            cmd.CommandText = ResultQuery;
            cmd.ExecuteNonQuery();
            //tran.Commit(); ///모든 정상 변경사항 트렌젝션 제출
            cmd.Transaction.Commit();
            //cmd.Dispose();
            Debug.Log("완료");
        }
        catch (Exception ex)
        {
            Debug.Log("DATABASE_INSERT ERROR : " + ex);
            tran.Rollback(); ///에러발생시 트렌젝션 롤백
        }
    }
    IEnumerator CopyDatabase()
    {
        string destinaionPath = Application.persistentDataPath + "/" + "AR_SEQ.db";

        //데이터 파일의 저장경로 설정해 둔것  

        //최초 어플실행시 로컬디바이스 경로에 DB 복사
        if (!File.Exists(destinaionPath))
        {
            UnityWebRequest uwr = new UnityWebRequest(Application.streamingAssetsPath + "/AR_SEQ.db");
            uwr.downloadHandler = new DownloadHandlerBuffer();

            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
                yield break;
            }
            else
            {
                File.WriteAllBytes(destinaionPath, uwr.downloadHandler.data);
                //dbPath = "URI=file:" + destinaionPath;
            }
        }
        dbPath = "URI=file:" + destinaionPath;
    }
    #endregion
}