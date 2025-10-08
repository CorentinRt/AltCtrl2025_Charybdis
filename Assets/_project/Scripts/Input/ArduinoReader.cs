using System;
using System.Collections.Concurrent;
using System.IO.Ports;
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
                _arduino = new SerialPort(_portName, _baudRate);
                _arduino.Open();
                _keepReading = true;
                _readThread = new Thread(ReadSerial);
                _readThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("Impossible d'ouvrir le port Arduino : " + e.Message);
            }
        }

        private void ReadSerial()
        {
            while (_keepReading)
            {
                try
                {
                    string message = _arduino.ReadLine();
                    _messageQueue.Enqueue(message);
                }
                catch (TimeoutException)
                {
                    // Ignore
                }
                catch (Exception e)
                {
                    Debug.LogError("Erreur lecture Arduino : " + e.Message);
                }
            }
        }

        void Update()
        {
            while (_messageQueue.TryDequeue(out string message))
            {
                ProcessMessage(message);
            }
        }

        private void ProcessMessage(string message)
        {
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
        }

        void OnApplicationQuit()
        {
            _keepReading = false;
            if (_readThread != null && _readThread.IsAlive)
                _readThread.Join();

            if (_arduino != null && _arduino.IsOpen)
                _arduino.Close();
        }
    }
}
