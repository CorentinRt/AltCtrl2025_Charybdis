using System;
using System.IO.Ports;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ArduinoReader : MonoBehaviour
    {
        private SerialPort _arduino;
        [SerializeField] private string _portName = "COM3";
        [SerializeField] private int _baudRate = 9600;

        public event Action<int> OnRotator1Move;
        public event Action<int> OnRotator2Move;

        void Start()
        {
            _arduino = new SerialPort(_portName, _baudRate);
            try
            {
                _arduino.Open();
                _arduino.ReadTimeout = 50;
            }
            catch (Exception e)
            {
                Debug.LogError("Impossible d'ouvrir le port Arduino : " + e.Message);
            }
        }

        void Update()
        {
            if (_arduino == null || !_arduino.IsOpen) return;

            try
            {
                string message = _arduino.ReadLine();

                // Exemple de message : "Encodeur 1: 1"
                if (message.StartsWith("Encodeur 1:"))
                {
                    string valueStr = message.Replace("Encodeur 1:", "").Trim();
                    if (int.TryParse(valueStr, out int dir))
                        OnRotator1Move?.Invoke(dir);
                }
                else if (message.StartsWith("Encodeur 2:"))
                {
                    string valueStr = message.Replace("Encodeur 2:", "").Trim();
                    if (int.TryParse(valueStr, out int dir))
                        OnRotator2Move?.Invoke(dir);
                }
                else if (message.Contains("Bouton Encodeur 1"))
                {
                    Debug.Log("Bouton Encodeur 1 pressé");
                }
                else if (message.Contains("Bouton Encodeur 2"))
                {
                    Debug.Log("Bouton Encodeur 2 pressé");
                }
            }
            catch (TimeoutException)
            {
                // ignore
            }
        }

        void OnApplicationQuit()
        {
            if (_arduino != null && _arduino.IsOpen)
                _arduino.Close();
        }
    }
}
