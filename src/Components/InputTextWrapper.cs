using TMPro;
using UnityEngine;

namespace ModConfigMenu.Components
{
    [RequireComponent(typeof(TMPro.TMP_InputField))]
    public class InputTextWrapper : MonoBehaviour
    {
        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField = gameObject.GetComponent<TMP_InputField>();
        }

        private void OnEnable()
        {
            //inputField.ActivateInputField();
        }
        
        private void OnDisable()
        {
            _inputField.DeactivateInputField();
        }
    }
}