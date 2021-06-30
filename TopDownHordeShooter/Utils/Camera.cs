using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownHordeShooter.Utils
{
    /**
     * Extracted fom http://magellanicgames.co.uk/basic2dCamera.html
     */
    public class Camera
    {
        readonly Matrix      _mProjection;
        Matrix      _mView;

        // Position and up vector
        Vector3 _mPosition;
        readonly Vector3     _mUp;
        Vector3     _mCenter;

        readonly Vector3     _mOffset;
        
        public Camera(int width, int height, Vector3 pos, float zOffset = 10.0f)
        {    
            _mUp = Vector3.Up;
            _mPosition = pos;
            _mOffset = new Vector3(0,0,zOffset);
            _mCenter = _mPosition + _mOffset;

            var nearClip = 1.0f;
            var farClip = -50.0f;

            Matrix.CreateOrthographic(width, height, nearClip, farClip, out _mProjection);
            _mView = Matrix.CreateLookAt(_mPosition, _mCenter, _mUp);
        }

        public Vector3 GetPosition()
        {
            return _mPosition;
        }
        
        public void SetPosition(Vector3 pos)
        {
            _mPosition = pos;      
            _mCenter = _mPosition + _mOffset;
            Console.Write("Cam pos: " + _mPosition + "\n");
            _mView = Matrix.CreateLookAt(_mPosition, _mCenter, _mUp);
        }
        
        public void PassParameters(Effect effect)
        {
            effect.Parameters["View"].SetValue(_mView);
            effect.Parameters["Projection"].SetValue(_mProjection);      
        }
    }
}