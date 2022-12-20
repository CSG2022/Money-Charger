using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] public ClickerBehaviour _clickerBehaviour;
    [SerializeField] public GameObject Parent;
    [SerializeField]public float amountPerClick = .05f;
    bool _clickedReleased = true;
    [SerializeField] public int Level = 0;
    public bool IsDraggingToMergeOrMove;
    public bool ReadyToMerge;
    GameObject _mergingInto;
    [SerializeField]public GameObject UpgradeGraphic;
    [SerializeField]public GameObject CantUpgradeGraphic;
    public bool IsSwitchNew;
    public bool Working;
    public MeshRenderer CableMeshRenderer;
    [SerializeField] Material[] _activeCableMaterials;
    [SerializeField] Material[] _inactiveCableMaterials;
    [SerializeField,HideInInspector]public MeshFilter SwitchMeshFilter;
    [SerializeField]public MeshRenderer BoxMeshRenderer;

    void Start()
    {
        if( _clickerBehaviour == null )
        {
           _clickerBehaviour = GameObject.FindGameObjectWithTag("GlobalScripts").GetComponent<ClickerBehaviour>();
        }
        SwitchMeshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        if(IsDraggingToMergeOrMove && IsSwitchNew)
        {
            GameManager.draggingNewSwitch = true;
        }
        else if(IsDraggingToMergeOrMove && !IsSwitchNew)
        {
            GameManager.draggingNewSwitch = false;
        }
        if(IsDraggingToMergeOrMove && !ReadyToMerge)
        {
            if (_mergingInto != null)
            {
            var btn = _mergingInto.GetComponent<SwitchButton>();
                btn.CantUpgradeGraphic.SetActive(false);
            }
            _mergingInto = null;
        }
        if(!IsDraggingToMergeOrMove && ReadyToMerge)
        {
            Merge(_mergingInto);
        }
    }

    private void OnMouseDown()
    {
        // make sure it doesnt drop down 2 times in a row
        if(_clickedReleased)
        {
            if (!GameManager.ButtonFrienzy)
                base.transform.position = new Vector3(transform.position.x, transform.position.y - .06f, transform.position.z);

            if(GameManager.ButtonFrienzy)
            {
                for(int i = 0; i < GameManager.SwitchButtonParentsActive.Length;i++)
                {
                    _clickerBehaviour.IncrementByAmount(amountPerClick);
                    var parent = GameManager.SwitchButtonParentsActive[i];
                    parent._clickableButton.transform.position = new Vector3(parent._clickableButton.transform.position.x, parent._clickableButton.transform.position.y - .06f, parent._clickableButton.transform.position.z);
                    parent._clickableButton._clickedReleased = false;
                }
            }
            else if(Working)
                _clickerBehaviour.IncrementByAmount(amountPerClick);
            _clickedReleased = false;
        }
    }
    private void OnMouseUp()
    {
        if (GameManager.ButtonFrienzy)
        {
            for (int i = 0; i < GameManager.SwitchButtonParentsActive.Length; i++)
            {
                var parent = GameManager.SwitchButtonParentsActive[i];
                parent._clickableButton.transform.position = new Vector3(parent._clickableButton.transform.position.x, parent._clickableButton.transform.position.y + .06f, parent._clickableButton.transform.position.z);
                parent._clickableButton._clickedReleased = true;
            }
        }
        else
            base.transform.position = new Vector3(transform.position.x, transform.position.y + .06f, transform.position.z);

        _clickedReleased = true;
    }

    private void OnTriggerStay(Collider collision)
    {
        if (IsDraggingToMergeOrMove && collision.gameObject.tag == "ButtonBox")
        {
            // Try to merge 
            int otherSwitchButtonLevel = collision.gameObject.GetComponent<SwitchButton>().Level;
            if (Level == otherSwitchButtonLevel)
            {
                ReadyToMerge = true;
                
                _mergingInto = collision.gameObject.GetComponent<SwitchButton>().Parent;
                collision.gameObject.GetComponent<SwitchButton>().CantUpgradeGraphic.SetActive(false);
                collision.gameObject.GetComponent<SwitchButton>().UpgradeGraphic.SetActive(true);
            }
            else
            {
                _mergingInto = collision.gameObject.GetComponent<SwitchButton>().Parent;
                collision.gameObject.GetComponent<SwitchButton>().UpgradeGraphic.SetActive(false);
                collision.gameObject.GetComponent<SwitchButton>().CantUpgradeGraphic.SetActive(true);

                //_mergingInto = null;
                ReadyToMerge = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(_mergingInto != null)
        {
            _mergingInto.GetComponentInChildren<SwitchButton>().CantUpgradeGraphic.SetActive(false);
            _mergingInto.GetComponentInChildren<SwitchButton>().UpgradeGraphic.SetActive(false);
        }
        ReadyToMerge = false;
        _mergingInto = null;
    }

    void Merge(GameObject buttonsParent)
    {
        // Change material, change Meshes

        GameManager.SwitchButtonSound.Play();
        _mergingInto.GetComponentInChildren<SwitchButton>().UpgradeGraphic.SetActive(false);
        var btn = buttonsParent.GetComponentInChildren<SwitchButton>();
        btn.Level++;
        btn.amountPerClick += .05f; // Discuss the upgrade amount and put it into a variable
        if(btn.Level == 0)
        {
            btn.BoxMeshRenderer.materials = GameManager.BoxLevel1Materials;
            btn.SwitchMeshFilter.mesh = _clickerBehaviour.Lvl1SwitchMesh;
        }
        else if(btn.Level == 1)
        {
            btn.BoxMeshRenderer.materials = GameManager.BoxLevel2Materials;
            btn.SwitchMeshFilter.mesh = _clickerBehaviour.Lvl2SwitchMesh;
        }
        else if(btn.Level == 2)
        {
            btn.BoxMeshRenderer.materials = GameManager.BoxLevel3Materials;
            btn.SwitchMeshFilter.mesh = _clickerBehaviour.Lvl3SwitchMesh;
        }

        CableMeshRenderer.materials = _inactiveCableMaterials;
        GameManager.SwitchButtonParentsActive = GameManager.SwitchButtonParentsActive.Where(x => x._clickableButton != this).ToArray();
        Parent.transform.position = Parent.GetComponent<Draggable>()._startingPosition;
        ReadyToMerge = false;
        if (Working)
        {
            btn.GetComponent<MeshRenderer>().material = GameManager.ButtonActiveMaterial;
            btn.CableMeshRenderer.materials = _activeCableMaterials;
            btn.Working = true;
        }
        Parent.SetActive(false);
    }
}
