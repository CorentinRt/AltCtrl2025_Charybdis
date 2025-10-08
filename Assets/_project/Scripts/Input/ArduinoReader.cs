using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ArduinoReader : MonoBehaviour
    {
        // ----- FIELDS ----- //
        private SerialPort _arduino;
        private Thread _readThread;
        private bool _keepReading;

        private readonly ConcurrentQueue<string> _messageQueue = new();

        [SerializeField] private string _portName = "COM3";
        [SerializeField] private int _baudRate = 9600;

        public event Action<int> OnRotator1Move;
        public event Action<int> OnRotator2Move;
        // ----- FIELDS ----- //

        void Start()
        {
            try
            {
                if (!SerialPort.GetPortNames().Contains(_portName))
                {
                    Debug.LogError($"Le port série '{_portName}' n'existe pas.");
                    return;
                }

                _arduino = new SerialPort(_portName, _baudRate)
                {
                    ReadTimeout = 100,
                    DtrEnable = true,
                    RtsEnable = true
                };
                _arduino.Open();

                _keepReading = true;
                _readThread = new Thread(ReadSerial)
                {
                    IsBackground = true
                };
                _readThread.Start();

                Debug.Log("Connexion à l'Arduino réussie.");
            }
            catch (Exception e)
            {
                Debug.LogError("Erreur d'initialisation Arduino : " + e.Message);
            }
        }

        private void ReadSerial()
        {
            try
            {
                while (_keepReading)
                {
                    if (_arduino == null || !_arduino.IsOpen)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    try
                    {
                        string message = _arduino.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(message))
                        {
                            _messageQueue.Enqueue(message);
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Ignorer, comportement normal
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Erreur lors de la lecture série : " + e.Message);
                    }

                    Thread.Sleep(1); // éviter de surcharger le CPU
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Thread Arduino crashé : " + e.Message);
            }
        }

        void Update()
        {
            while (_messageQueue.TryDequeue(out string message))
            {
                try
                {
                    ProcessMessage(message);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Erreur dans le traitement du message Arduino : " + e.Message);
                }
            }
        }

        private void ProcessMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (message.StartsWith("Encodeur 1:"))
            {
                if (int.TryParse(message.Replace("Encodeur 1:", "").Trim(), out int dir))
                    OnRotator1Move?.Invoke(dir);
            }
            else if (message.StartsWith("Encodeur 2:"))
            {
                if (int.TryParse(message.Replace("Encodeur 2:", "").Trim(), out int dir))
                    OnRotator2Move?.Invoke(dir);
            }
            else
            {
                Debug.Log($"Message inconnu de l'Arduino : {message}");
            }
        }

        void OnApplicationQuit()
        {
            _keepReading = false;

            try
            {
                _readThread?.Join(200);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Erreur lors de l'arrêt du thread Arduino : " + e.Message);
            }

            try
            {
                if (_arduino != null && _arduino.IsOpen)
                    _arduino.Close();
            }
            catch (Exception e)
            {
                Debug.LogWarning("Erreur lors de la fermeture du port série : " + e.Message);
            }

            _arduino = null;
        }
    }
}
