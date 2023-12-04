using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerLogic : MonoBehaviour
{
    CharacterController m_characterController;

    float m_movementSpeed = 5.0f;

    float m_horizontalInput;
    float m_verticalInput;

    Vector3 m_movementInput;
    Vector3 m_movement;

    float m_jumpHeight = 0.6f;
    float m_gravity = 0.05f;
    bool m_jump = false;

    GameObject m_interactiveObject = null;

    GameObject m_equippedObject = null;

    [SerializeField]
    Transform m_weaponEquipmentPosition;

    int m_playerHealth = 100;

    [SerializeField]
    TMP_Text m_healthText;

    // Start is called before the first frame update
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        SetHealthText();
    }

    void SetHealthText()
    {
        if(m_healthText)
        {
            m_healthText.text = "Health: " + m_playerHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");

        m_movementInput = new Vector3(m_horizontalInput, 0, m_verticalInput);

        if(!m_jump && Input.GetButtonDown("Jump"))
        {
            m_jump = true;
        }

        
        
    }

    void RotateCharacterTowardsMouseCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = mousePos - playerPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);
    }

    void FixedUpdate()
    {
        m_movement = m_movementInput * m_movementSpeed * Time.deltaTime;

        RotateCharacterTowardsMouseCursor();

        // Face movement direction
        /*if(m_movementInput != Vector3.zero)
        {
            transform.forward = Quaternion.Euler(0, -90, 0) * m_movementInput.normalized;
        }*/

        // Apply gravity
        if (!m_characterController.isGrounded)
        {
            if(m_movement.y > 0)
            {
                m_movement.y -= m_gravity;
            }
            else
            {
                m_movement.y -= m_gravity * 1.5f;
            }
        }
        else
        {
            m_movement.y = 0;
        }

        // Setting jumpheight to movement y
        if(m_jump)
        {
            m_movement.y = m_jumpHeight;
            m_jump = false;
        }

        if(m_characterController)
        {
            m_characterController.Move(m_movement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            m_interactiveObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" && m_interactiveObject == other.gameObject)
        {
            m_interactiveObject = null;
        }
    }

    public void TakeDamage(int damage)
    {
        m_playerHealth -= damage;
        SetHealthText();

        if(m_playerHealth <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
