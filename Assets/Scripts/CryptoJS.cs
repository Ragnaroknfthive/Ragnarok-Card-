using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Derives a key from a password using an OpenSSL-compatible version of the PBKDF1 algorithm.
/// </summary>
/// <remarks>
/// based on the OpenSSL EVP_BytesToKey method for generating key and iv
/// http://www.openssl.org/docs/crypto/EVP_BytesToKey.html
/// </remarks>
public class OpenSslCompatDeriveBytes : DeriveBytes
{
    private readonly byte[] _data;
    private readonly HashAlgorithm _hash;
    private readonly int _iterations;
    private readonly byte[] _salt;
    private byte[] _currentHash;
    private int _hashListReadIndex;
    private List<byte> _hashList;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSslCompat.OpenSslCompatDeriveBytes"/> class specifying the password, key salt, hash name, and iterations to use to derive the key.
    /// </summary>
    /// <param name="password">The password for which to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key.</param>
    /// <param name="hashName">The name of the hash algorithm for the operation. (e.g. MD5 or SHA1)</param>
    /// <param name="iterations">The number of iterations for the operation.</param>
    public OpenSslCompatDeriveBytes(string password, byte[] salt, string hashName, int iterations)
        : this(new UTF8Encoding(false).GetBytes(password), salt, hashName, iterations)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSslCompat.OpenSslCompatDeriveBytes"/> class specifying the password, key salt, hash name, and iterations to use to derive the key.
    /// </summary>
    /// <param name="password">The password for which to derive the key.</param>
    /// <param name="salt">The key salt to use to derive the key.</param>
    /// <param name="hashName">The name of the hash algorithm for the operation. (e.g. MD5 or SHA1)</param>
    /// <param name="iterations">The number of iterations for the operation.</param>
    public OpenSslCompatDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations)
    {
        if (iterations <= 0)
            throw new ArgumentOutOfRangeException("iterations", iterations, "iterations is out of range. Positive number required");

        _data = password;
        _salt = salt;
        _hash = HashAlgorithm.Create(hashName);
        _iterations = iterations;
    }

    /// <summary>
    /// Returns a pseudo-random key from a password, salt and iteration count.
    /// </summary>
    /// <param name="cb">The number of pseudo-random key bytes to generate.</param>
    /// <returns>A byte array filled with pseudo-random key bytes.</returns>
    public override byte[] GetBytes(int cb)
    {
        if (cb <= 0)
            throw new ArgumentOutOfRangeException("cb", cb, "cb is out of range. Positive number required.");

        if (_currentHash == null)
        {
            _hashList = new List<byte>();
            _currentHash = new byte[0];
            _hashListReadIndex = 0;

            int preHashLength = _data.Length + ((_salt != null) ? _salt.Length : 0);
            var preHash = new byte[preHashLength];

            Buffer.BlockCopy(_data, 0, preHash, 0, _data.Length);
            if (_salt != null)
                Buffer.BlockCopy(_salt, 0, preHash, _data.Length, _salt.Length);

            _currentHash = _hash.ComputeHash(preHash);

            for (int i = 1; i < _iterations; i++)
            {
                _currentHash = _hash.ComputeHash(_currentHash);
            }

            _hashList.AddRange(_currentHash);
        }

        while (_hashList.Count < (cb + _hashListReadIndex))
        {
            int preHashLength = _currentHash.Length + _data.Length + ((_salt != null) ? _salt.Length : 0);
            var preHash = new byte[preHashLength];

            Buffer.BlockCopy(_currentHash, 0, preHash, 0, _currentHash.Length);
            Buffer.BlockCopy(_data, 0, preHash, _currentHash.Length, _data.Length);
            if (_salt != null)
                Buffer.BlockCopy(_salt, 0, preHash, _currentHash.Length + _data.Length, _salt.Length);

            _currentHash = _hash.ComputeHash(preHash);

            for (int i = 1; i < _iterations; i++)
            {
                _currentHash = _hash.ComputeHash(_currentHash);
            }

            _hashList.AddRange(_currentHash);
        }

        byte[] dst = new byte[cb];
        _hashList.CopyTo(_hashListReadIndex, dst, 0, cb);
        _hashListReadIndex += cb;

        return dst;
    }

    /// <summary>
    /// Resets the state of the operation.
    /// </summary>
    public override void Reset()
    {
        _hashListReadIndex = 0;
        _currentHash = null;
        _hashList = null;
    }
}

public static class CryptoJS
{
    #region Constants

    private const int SALT_SIZE = 8;
    private const int SIGNATURE_SIZE = 8;

    #endregion

    #region Variables
    private static readonly int[] _cryptoJsWordsSignature = { 0x53616c74, 0x65645f5f };

    private static readonly byte[] _signature = _cryptoJsWordsSignature
        .SelectMany(v =>
        {
            var byteArray = BitConverter.GetBytes(v);
            Array.Reverse(byteArray);
            return byteArray;
        }).ToArray();

    #endregion

    #region Private Methods
    private static (byte[], byte[]) ExtractSaltAndMsgInBytes(byte[] data)
    {
        var salt = new byte[SALT_SIZE];
        Array.Copy(data, SIGNATURE_SIZE, salt, 0, SALT_SIZE);
        var encryptedBytes = new byte[data.Length - SIGNATURE_SIZE - SALT_SIZE];
        Array.Copy(data, SIGNATURE_SIZE + SALT_SIZE, encryptedBytes, 0, encryptedBytes.Length);
        return (salt, encryptedBytes);
    }

    private static byte[] GenerateRandomSaltIfNeeded(byte[] salt)
    {
        if (salt.Length != 0)
        {
            return salt;
        }

        salt = new byte[SALT_SIZE];
        RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        rngCsp.GetBytes(salt);

        return salt;
    }

    private static void ConcatByteArrays(byte[] salt, byte[] result, byte[] data)
    {
        for (var i = 0; i < result.Length; i++)
        {
            if (i < SIGNATURE_SIZE)
            {
                result[i] = _signature[i];
            }
            else if (i < SIGNATURE_SIZE + SALT_SIZE)
            {
                result[i] = salt[i - 8];
            }
            else
            {
                result[i] = data[i - 16];
            }
        }
    }

    private static bool IsEncryptedByCryptoJs(byte[] dataArr)
    {
        if (dataArr.Length < _signature.Length) return false;

        for (var i = 0; i < _signature.Length; i++)
        {
            if (dataArr[i] != _signature[i])
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Public Methods
    public static string btoa(string str)
    {
        return Convert.ToBase64String(Encoding.GetEncoding(28591).GetBytes(str));
    }

    public static string Encrypt(string plainText, string secret, byte[] salt)
    {
        salt = GenerateRandomSaltIfNeeded(salt);

        var key = new OpenSslCompatDeriveBytes(secret, salt, "MD5", 1);
        Aes aesAlg = Aes.Create();
        aesAlg.Key = key.GetBytes(32);
        aesAlg.IV = key.GetBytes(16);

        MemoryStream encryptionStream = new MemoryStream();
        CryptoStream encrypt = new CryptoStream(encryptionStream,
            aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
        byte[] utfD = new UTF8Encoding(false).GetBytes(plainText);

        encrypt.Write(utfD, 0, utfD.Length);
        encrypt.FlushFinalBlock();
        encrypt.Close();
        byte[] data = encryptionStream.ToArray();

        var result = new byte[_signature.Length + salt.Length + data.Length];
        ConcatByteArrays(salt, result, data);

        key.Reset();

        return Convert.ToBase64String(result);
    }

    public static string Encrypt(string plainText, string secret)
    {
        return Encrypt(plainText, secret, Array.Empty<byte>());
    }

    public static string Decrypt(byte[] msg, string secret, byte[] salt)
    {
        if (!salt.Any())
        {
            throw new ArgumentException("Salt value is not provided");
        }

        try
        {
            var key = new OpenSslCompatDeriveBytes(secret, salt, "MD5", 1);
            Aes aesAlg = Aes.Create();
            aesAlg.Key = key.GetBytes(32);
            aesAlg.IV = key.GetBytes(16);

            MemoryStream decryptionStream = new MemoryStream();
            CryptoStream decrypt = new CryptoStream(decryptionStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Write);

            decrypt.Write(msg, 0, msg.Length);
            decrypt.Flush();
            decrypt.Close();
            key.Reset();

            return new UTF8Encoding(false).GetString(decryptionStream.ToArray());
        }
        catch (CryptographicException ex)
        {
            return string.Empty;
        }
    }

    public static string Decrypt(string plainText, string secret)
    {
        var data = Convert.FromBase64String(plainText);

        if (!IsEncryptedByCryptoJs(data))
        {
            throw new ArgumentException("This message was not encrypted by crypto-js");
        }

        var (salt, encryptedBytes) = ExtractSaltAndMsgInBytes(data);

        return Decrypt(encryptedBytes, secret, salt);
    }

    #endregion
}
