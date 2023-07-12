using UnityEngine;
using UnityEngine.Assertions;

namespace IV.Gameplay.GameCamera
{
    [ExecuteAlways]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 positionOffset = Vector3.zero;
        [Range(1f, 89f)] [SerializeField] private float pitchOffset = 45f;
        [Range(0, 360f)] [SerializeField] private float yawOffset = 0f;
        [Min(1f)] [SerializeField] private float armLength = 10f;
        [SerializeField] private float smoothSpeed = .125f;

        private Vector3 desiredPosition;
        private Vector3 cameraArm;
        private Matrix4x4 trs;

        private void OnEnable()
        {
            desiredPosition = transform.position;
            cameraArm = new Vector3(0f, 0f, -armLength);
            trs = CreateTRSMatrix();
        }

        private void OnValidate()
        {
            trs = CreateTRSMatrix();
        }

        public void AddRotation(float pitch, float yaw)
        {
            pitchOffset = Mathf.Clamp(pitchOffset + pitch, 1f, 89f);
            yawOffset = Mathf.Clamp(yawOffset + yaw, 0f, 360f);
            UpdateRotation();
        }

        private void LateUpdate()
        {
            if (target == null) return;

            var targetPosition = target.position;
            //UpdateTranslation(ref targetPosition);
            desiredPosition = trs.MultiplyPoint3x4(cameraArm); // Optimized for affine transformations (TRS)

            var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(target);
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(desiredPosition, (target.position - desiredPosition));
            Gizmos.DrawSphere(desiredPosition, 2);
        }

        // Matrix4x4.Translate(Vector3)
        public void UpdateTranslation(ref Vector3 targetPosition)
        {
            // Translaition
            var translation = targetPosition + positionOffset;

            // @formatter:off
            // TRS' Translation part
            trs.m03 = translation.x;
            trs.m13 = translation.y;
            trs.m23 = translation.z;
            // @formatter:on

            Assert.IsTrue(trs.ValidTRS());
        }

        // Matrix4x4.Rotate(Quaternion)
        public void UpdateRotation()
        {
            CalculateSinCos(pitchOffset, yawOffset, out var sinX, out var cosX, out var sinY, out var cosY);

            // @formatter:off
            // Rotation (YX) * Scale(1)
            trs.m00 = cosY;  trs.m01 = sinY * sinX; trs.m02 = sinY * cosX;
            trs.m10 = 0;     trs.m11 = cosX;        trs.m12 = -sinX;
            trs.m20 = -sinY; trs.m21 = cosY * sinX; trs.m22 = cosY * cosX;
            // @formatter:on

            Assert.IsTrue(trs.ValidTRS());
        }

        // It's Matrix4x4.TRS(translation, Quaternion.Euler(0f, pitchOffset, yawOffset), Vector3.one);
        private Matrix4x4 CreateTRSMatrix()
        {
            CalculateSinCos(pitchOffset, yawOffset, out var sinX, out var cosX, out var sinY, out var cosY);

            // Translaition
            var translation = positionOffset; // don't account for target position just yet

            // TRS
            var trsMatrix = new Matrix4x4 // @formatter:off
            {
                // Rotation (YX) * Scale(1)                        // Translation part
                m00 = cosY,  m01 = sinY * sinX, m02 = sinY * cosX, m03 = translation.x,
                m10 = 0,     m11 = cosX,        m12 = -sinX,       m13 = translation.y,
                m20 = -sinY, m21 = cosY * sinX, m22 = cosY * cosX, m23 = translation.z,
                m30 = 0,     m31 = 0,           m32 = 0,           m33 = 1
            };
            // @formatter:on

            Assert.IsTrue(trsMatrix.ValidTRS());

            return trsMatrix;
        }

        private static void CalculateSinCos(in float pitch, in float yaw, out float sinX, out float cosX, out float sinY, out float cosY)
        {
            // deg -> rad
            var x = pitch * Mathf.Deg2Rad;
            var y = yaw * Mathf.Deg2Rad;

            sinX = Mathf.Sin(x);
            cosX = Mathf.Cos(x);
            sinY = Mathf.Sin(y);
            cosY = Mathf.Cos(y);
        }
    }
}