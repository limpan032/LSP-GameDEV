using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints; // массив точек пути
    private int currentWaypoint = 0; // текущая цель

    [SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;

    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;

    [SerializeField] private float _force;
    [SerializeField] private float _maxAngle;
    [SerializeField] private float _maxDistanceToWaypoint = 5f; // расстояние, после которого бот меняет цель
    [SerializeField] private float _steerSensitivity = 1f; // чувствительность руля

    private void FixedUpdate()
    {
        // Движение к текущей точке пути
        Vector3 directionToWaypoint = waypoints[currentWaypoint].position - transform.position;
        float distanceToWaypoint = directionToWaypoint.magnitude;

        // Отладочное сообщение для проверки расстояния до точки пути
        Debug.Log("Distance to waypoint: " + distanceToWaypoint);

        // Нормализуем направление
        directionToWaypoint.Normalize();

        // Вычисляем локальное направление (в координатах машины)
        Vector3 localDirection = transform.InverseTransformDirection(directionToWaypoint);

        // Определяем, вперед или назад нужно двигаться
        float moveDirection = Mathf.Sign(localDirection.z); // если точка впереди, будет 1, если позади -1

        // Вычисляем угол для поворота
        float steer = Mathf.Clamp(localDirection.x * 5f, -1f, 1f); // Угол поворота от -1 до 1

        // Отладочное сообщение для проверки угла поворота
        Debug.Log("Steer Angle: " + steer);
        _colliderFL.steerAngle = _maxAngle * steer;
        _colliderFR.steerAngle = _maxAngle * steer;

        // Движение вперед или назад
        _colliderFL.motorTorque = -_force * moveDirection;
        _colliderFR.motorTorque = -_force * moveDirection;

        // Если машина достаточно близко к точке, выбираем следующую точку
        if (distanceToWaypoint < _maxDistanceToWaypoint)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        // Синхронизируем вращение колес
        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR);
    }

    private void RotateWheel(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        transform.position = position;
        transform.rotation = rotation;
    }

}
