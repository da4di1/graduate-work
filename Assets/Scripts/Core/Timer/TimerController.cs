using System;
using Core.Services.Updater;
using TMPro;
using UnityEngine;

namespace Core.Timer
{
    public class TimerController : IDisposable
    {
        private readonly TextMeshProUGUI _currentTimeText;
        private float _currentTime;

        public event Action TimeExpired;
        
        public TimerController(int timeInMinutes, TextMeshProUGUI currentTimeText)
        {
            _currentTimeText = currentTimeText;
            _currentTime = timeInMinutes * 60;

            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public void Dispose()
        {
            ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (ProjectUpdater.Instance.IsPaused) return;
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                TimeExpired?.Invoke();
            }
            TimeSpan formatedTime = TimeSpan.FromSeconds(_currentTime);
            _currentTimeText.text = formatedTime.Minutes.ToString() + ":" + formatedTime.Seconds.ToString();
        }
    }
}