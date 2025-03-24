using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using GameDevTV.Inventories;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        ActionStore actionStore;

        [System.Serializable]
        struct CusorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CusorMapping[] cusorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        [SerializeField] int numberOfAbilities = 6;

        float raycastRadiusWorld = 0f;
        float raycastRadiusUI = 0.1f;
        bool isDraggingUI = false;

        void Awake()
        {
            health = GetComponent<Health>();
            actionStore = GetComponent<ActionStore>();
            raycastRadiusWorld = raycastRadius;
        }

        void Update()
        {
            if (InteractWithUI()) { return; }

            if(health.IsDead()) 
            {
                SetCursor(CursorType.None);                  
                return;
            }

            UseAbilities();

            if (InteractWithComponent()) { return; }
            if(InteractWithMovement()) { return; }

            SetCursor(CursorType.None);
        }

        bool InteractWithUI()
        {
            if(Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            if(EventSystem.current.IsPointerOverGameObject())
            {
                if(Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }

                raycastRadius = raycastRadiusUI;
                SetCursor(CursorType.UI);
                return true;
            }

            raycastRadius = raycastRadiusWorld;

            if(isDraggingUI)
            {
                return true;
            }
            return false;
        }

        void UseAbilities()
        {
            for(int i = 0; i < numberOfAbilities; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1 + i))
                {                   
                    actionStore.Use(i, gameObject);
                }
            }
        }

        bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();

            foreach(RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for(int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if(hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) { return false; }

                if(Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);                  
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(!hasHit) { return false; }

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh) { return false; }

            target = navMeshHit.position;

            return true;
        }

        public void SetCursor(CursorType type)
        {
            CusorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        CusorMapping GetCursorMapping(CursorType type)
        {
            foreach(CusorMapping mapping in cusorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cusorMappings[0];
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        } 
    }
}
