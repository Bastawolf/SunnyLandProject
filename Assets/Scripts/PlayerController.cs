using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Поля для Start()
    private Animator anim;
    private Collider2D cell;
    private Rigidbody2D rb2d;

    // Конечный автомат (Finite State Machine)
    private enum State { idle, running, jumping, falling, hurt }
    private State state = State.idle;

    // Поля для инспектора Unity
    [SerializeField] private Text cherryText;
    [SerializeField] private int cherries = 0;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float hurtForce = 10f;

    // Точка входа игры
    private void Start()
    {
        anim = GetComponent<Animator>();
        cell = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Метод, вызывающийся на каждом кадре (фрейме)
    private void Update()
    {
        // Для блокировки движения во время ранения
        if (state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); // Запуск анимаций (Команда, задающая 
                                              // анимацию исходя из состояния enum'а)
    }

    // Сбор вишенок
    private void OnTriggerEnter2D(Collider2D cherry)
    {
        // Если игрок столкнулся с вишенкой
        if (cherry.CompareTag("Collectable"))
        {
            // Убрать вишню с экрана
            Destroy(cherry.gameObject);

            // Увеличить счетчик вишенок
            cherries++;

            // Показать кол-во вишенок на экране
            cherryText.text = cherries.ToString() + "/6";
        }
    }

    // Все, что связано с лягушкой
    private void OnCollisionEnter2D(Collision2D frog)
    {
        // Если игрок столкнулся с лягушкой
        if (frog.gameObject.CompareTag("Enemy"))
        {
            Frog frogClass = frog.gameObject.GetComponent<Frog>();

            // Если игрок прыгнул на лягушку
            if (state == State.falling)
            {
                frogClass.JumpedOn();

                // Отпрыгиваем от нее
                Jump();
            }
            // В остальных случаях
            else
            {
                // Лягушка ранит игрока
                state = State.hurt;

                // Если лягушка справа от игрока
                if (frog.gameObject.transform.position.x > transform.position.x)
                {
                    // Его отбрасывает влево
                    rb2d.velocity = new Vector2(-hurtForce, rb2d.velocity.y);
                }
                // Иначе лягушка слева от игрока
                else
                {
                    // Его отбрасывает враво
                    rb2d.velocity = new Vector2(hurtForce, rb2d.velocity.y);
                }
            }
        }
    }

    // Перемещение
    private void Movement()
    {
        /* Создаем переменную, в которую записывается, какая нажата
        кнопка, отвечающая за горизонтальное перемещение */
        var hDirection = Input.GetAxis("Horizontal");

        // Если нажата кнопка, перемещающая игрока 
        // по горизонтальной оси влево
        if (hDirection < 0)
        {
            // Задаем отрицательную скорость по оси "x"
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);

            // Игрок смотрит влево
            transform.localScale = new Vector2(-1, 1);
        }
        // Если нажата кнопка, перемещающая игрока 
        // по горизонтальной оси вправо
        else if (hDirection > 0)
        {
            // Задаем положительную скорость по оси "x"
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);

            // Игрок смотрит вправо
            transform.localScale = new Vector2(1, 1);
        }

        // Если нажата кнопка, овечающая за прыжок, и игрок находится на земле
        if (Input.GetButtonDown("Jump") && cell.IsTouchingLayers(ground))
        {
            // Вызываем метод, отвечающий за прыжок
            Jump();
        }
    }

    // Прыжок
    private void Jump()
    {
        // Задаем скорость по оси "y"
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);

        // Задаем state jumping
        state = State.jumping;
    }

    // Заготовка анимаций (Метод, задающий состояния state для анимаций)
    private void AnimationState()
    {
        /* switch - case вместо иф */
        // Если игрок падает, задаем state falling
        if (state == State.jumping)
        {
            /* rb2d.velocity.y < 0.1f ?  state = State.falling; */
            if (rb2d.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        // Если игрок после падения приземлился на землю, задаем state idle
        else if(state == State.falling)
        {
            if (cell.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        // После ранения игрока, задаем state idle
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb2d.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        // Если игрок бежит, задаем state running
        else if (Mathf.Abs(rb2d.velocity.x) > 2f)
        {
            state = State.running;
        }
        // Во всех остальных случаях задаем state idle
        else
        {
            state = State.idle;
        }
    }
}