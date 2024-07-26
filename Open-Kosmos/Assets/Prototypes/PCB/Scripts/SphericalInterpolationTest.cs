using Kosmos.Prototypes.PCB.Math;
using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Prototypes.PCB
{
    [ExecuteAlways]
    public class SphericalInterpolationTest : MonoBehaviour
    {
        public SphericalCoordinateDegrees a;
        public SphericalCoordinateDegrees b;

        [Range((float)0.0, (float)1.0)]
        public double delta = 0.5;
        
        private void Update()
        {
            double3 aCartesian = this.a.ToCartesian();
            double3 bCartesian = this.b.ToCartesian();
            
            SphericalCoordinateDegrees interpolated = SphericalCoordinateDegrees.Interpolate(this.a, this.b, this.delta);

            double3 interpolatedCartesian = interpolated.ToCartesian();

            Vector3 aPoint = new Vector3((float) aCartesian.x, (float) aCartesian.y, (float) aCartesian.z);
            Vector3 bPoint = new Vector3((float) bCartesian.x, (float) bCartesian.y, (float) bCartesian.z);
            Vector3 interpolatedPoint = new Vector3((float) interpolatedCartesian.x, (float) interpolatedCartesian.y, (float) interpolatedCartesian.z);
            
            Debug.DrawLine(this.transform.position, this.transform.position + aPoint, Color.green);
            Debug.DrawLine(this.transform.position, this.transform.position + bPoint, Color.red);
            Debug.DrawLine(this.transform.position, this.transform.position + interpolatedPoint, Color.blue);
        }
    }
}