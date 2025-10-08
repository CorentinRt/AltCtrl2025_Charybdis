using System;
using System.IO.Ports;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    public class ArduinoReader : MonoBehaviour
    {
        // ----- FIELDS ----- //
        private SerialPort _arduino;
        [SerializeField] private string _portName = "COM3"; 
        [SerializeField] private int _baudRate = 9600;

        public event Action<int> OnRotator1Move;
        // ----- FIELDS ----- //

        void Start()
        {
            _arduino = new SerialPort(_portName, _baudRate);
            if (_arduino == null) return;

            _arduino.Open();
            _arduino.ReadTimeout = 50;  
        }

        void Update()
        {
            if (_arduino == null) return;

            if (_arduino.IsOpen)
            {
                try
                {
                    string message = _arduino.ReadLine();
                    int direction = int.Parse(message);
                    OnRotator1Move?.Invoke(direction);
                    //Debug.Log("Direction: " + direction);
                }
                catch (System.Exception)
                {
                    // Ignore timeout
                }
            }
        }

        void OnApplicationQuit()
        {
            if (_arduino != null && _arduino.IsOpen)
                _arduino.Close();
        }
    }
}
