using System;
using Unity.Mathematics;
using UnityEngine;

namespace CameraBehaviour.DataLayer.State
{
    [Serializable]
    public class CameraState : Clonable<CameraState>
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public float OrthographicSize;
        public bool IsEnabled;

        public CameraState() { }

        public CameraState(Vector3 pos, quaternion rot, float size, bool enabled)
        {
            Position = pos;
            Rotation = rot;
            OrthographicSize = size;
            IsEnabled = enabled;
        }

        public CameraState(Camera cam)
        {
            SetValue(cam);
        }

        public void SetValue(Camera cam)
        {
            if (cam == null) return;
            Position = cam.transform.position;
            Rotation = cam.transform.rotation;
            OrthographicSize = cam.orthographicSize;
            IsEnabled = cam.enabled;
        }

        public override CameraState DeepCopy()
        {
            var position = this.Position;
            var euler = this.Rotation.eulerAngles;
            return new CameraState
            {
                Position = new Vector3(position.x, position.y, position.z),
                Rotation = Quaternion.Euler(euler.x, euler.y, euler.z),
                OrthographicSize = this.OrthographicSize,
                IsEnabled = this.IsEnabled
            };
        }
    }
}