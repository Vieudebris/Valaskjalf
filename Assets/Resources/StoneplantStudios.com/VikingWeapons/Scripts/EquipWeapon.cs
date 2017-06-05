using System;
using UnityEngine;
using System.Collections;

namespace StoneplantStudios.VikingWeapons.Demo
{
    public class EquipWeapon : MonoBehaviour
    {
        [SerializeField]
        protected CharacterController.EquipLocation equipLocation = CharacterController.EquipLocation.Right;

        [SerializeField]
        protected CharacterController.EquipType equipType = CharacterController.EquipType.OneHanded;

        protected void OnMouseDown()
        {
            var c = FindObjectOfType<CharacterController>();
            var copy = Instantiate<EquipWeapon>(this);
            Destroy(copy.GetComponent<Collider>());

            c.Equip(copy.transform, equipLocation, equipType);
        }
    }
}