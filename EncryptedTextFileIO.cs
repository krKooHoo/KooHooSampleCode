using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptedTextFileIO : MonoBehaviour
{
    /// <summary>
    /// 그냥 플랫폼단 유저 ID 쓰는걸로
    /// </summary>
    public string FileFolderName;

    // 암호화를 위한 키와 초기화 벡터를 설정합니다.
    private string encryptionKey = "1234567890123456"; // 반드시 16자 이상의 키를 사용. 테스트로 넣어놓음
    private string initializationVector = "1234567890123456"; // 반드시 16자 이상의 IV를 사용. 테스트로 넣어놓음

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         SaveEncryptedTextToFile("물론! AWS(아마존 웹 서비스) 서포트 플랜은 AWS 고객이 AWS 클라우드 서비스를 사용하는 동안 지원을 받을 수 있는 유지보수 계획입니다. 주요 서포트 플랜 유형은 다음과 같습니다");
    //     }
    //     
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //         var result = LoadEncryptedTextFromFile();
    //         Debug.Log(result);
    //     }
    // }

    // 문자열을 암호화하여 파일에 저장
    public void SaveEncryptedTextToFile(string fileName, string textToSave)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, FileFolderName);

        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        
        string filePath = Path.Combine(Application.persistentDataPath, FileFolderName,$"{fileName}.txt");
        
        try
        {
            byte[] encryptedBytes = EncryptStringToBytes(textToSave, encryptionKey, initializationVector);

            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                fileStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
            Debug.Log("암호화된 텍스트 파일 저장 완료!");
        }
        catch (Exception e)
        {
            Debug.LogError("암호화된 텍스트 파일 저장 중 오류 발생: " + e.Message);
        }
    }

    // 암호화된 파일에서 문자열을 복호화. 에러시 null 반환. USE 베타 끝나고 수정해야할지도?
    public string LoadEncryptedTextFromFile(string fileName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, FileFolderName);

        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        
        string filePath = Path.Combine(Application.persistentDataPath, FileFolderName,$"{fileName}.txt");
        
        string loadedText = "";

        try
        {
            byte[] encryptedBytes;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                encryptedBytes = new byte[fileStream.Length];
                fileStream.Read(encryptedBytes, 0, encryptedBytes.Length);
            }

            loadedText = DecryptBytesToString(encryptedBytes, encryptionKey, initializationVector);
            Debug.Log("암호화된 텍스트 파일 읽기 완료!");
        }
        catch (Exception e)
        {
            Debug.LogWarning("암호화된 텍스트 파일 읽기 중 오류 발생 Null을 반환합니다 :  " + e.Message);
            loadedText = null;
        }

        return loadedText;
    }

    // 문자열을 바이트 배열로 암호화.
    private byte[] EncryptStringToBytes(string plainText, string key, string iv)
    {
        byte[] encrypted;

        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return encrypted;
    }

    // 바이트 배열을 문자열로 복호화.
    private string DecryptBytesToString(byte[] cipherBytes, string key, string iv)
    {
        string plaintext = null;

        using (AesManaged aesAlg = new AesManaged())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
