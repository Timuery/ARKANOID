using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public List<ActiveEffect> activeEffects = new List<ActiveEffect>(); // Активные эффекты на шаре
    private EffectController effectController;
    public GameObject ballPrefab; // Префаб шара для размножения
    public float baseSpeed = 5f;
    public int baseDamage = 1;
    public float currentSpeed;
    public int currentDamage;
    public Vector3 originalScale;
    private SpriteRenderer ballRenderer; // Для изменения текстуры
    private Rigidbody2D rb2d;
    private Vector2 direct;
    public Sprite BaseSprite;

    private AudioSource ballSource;

    private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        effectController = FindObjectOfType<EffectController>();
        currentSpeed = baseSpeed;
        currentDamage = baseDamage;
        originalScale = ballPrefab.transform.lossyScale;
        Debug.Log("Scale"   + originalScale.ToString());
        ballRenderer = GetComponent<SpriteRenderer>();

        ballSource = GetComponent<AudioSource>();
        Debug.Log("ball is Spawned");
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
    void StartSpeed()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.up * baseSpeed;
        
    }
    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0) && transform.parent != null && !controller.gameIsPause)
        {
            StartSpeed();
            controller.gameIsStarted = true;
            transform.parent = null;
        }
        if (Input.GetMouseButtonDown(1) && transform.parent == null && !controller.gameIsPause)
        {
            controller.Destroyed();
            controller.FindPaddleAndBall();
            controller.hpcontroller(-1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseClick();
        UpdateEffects();
        updateSpeed();
    }
    public void updateSpeed()
    {
        try
        {
            Vector2 currentDirection = rb2d.velocity.normalized;
            // Применяем новую скорость без изменения направления
            rb2d.velocity = currentDirection * currentSpeed;
        }
        catch
        {

        }
    }
    private void UpdateEffects()
    {
        Debug.Log(activeEffects.Count);
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            // Уменьшаем оставшееся время
            activeEffects[i].remainingDuration -= Time.deltaTime;

            // Применяем эффект
            ApplyEffect(activeEffects[i].effectData);

            // Удаляем эффект, если его время истекло
            /*if (activeEffects.Count < 0)
            {
                ballRenderer.sprite = ballPrefab.GetComponent<SpriteRenderer>().sprite;
                transform.localScale = originalScale;
                currentSpeed = baseSpeed;
                currentDamage = baseDamage;
            }*/
            if (activeEffects[i].remainingDuration < 0)
            {
                Debug.Log("I: " + i.ToString() + " | CountEffects: " + activeEffects.Count + " | activeEffect: " + activeEffects);
                RemoveEffect(activeEffects[i].effectData);
                //activeEffects.RemoveAt(i-1);
            }

        }
    }
    private void ApplyEffect(EffectData effect)
    {
        // Применяем текстуру эффекта, если она есть
        if (effect.newTexture != null)
        {
            ballRenderer.sprite = effect.newTexture;
        }


        // Применяем размер
        if (effect.sizeMultiplier != 1)
        {
            transform.localScale = originalScale * effect.sizeMultiplier;
        }

        // Применяем скорость
        if (effect.speedMultiplier != 1)
        {
            currentSpeed = baseSpeed * effect.speedMultiplier;
        }

        // Применяем урон
        if (effect.damage > 1)
        {
            currentDamage = effect.damage;
        }
        if (effect.damage <= 0 & effect.damage > -1)
        {
            currentDamage = 0;
        }
    }
    private void RemoveEffect(EffectData effect)
    {
        activeEffects.RemoveAll(e => e.effectData == effect);

        // Сбрасываем параметры к базовым значениям
        if (activeEffects.Count == 0)
        {
            ballRenderer.sprite = BaseSprite;
            transform.localScale = originalScale;
            currentSpeed = baseSpeed;
            currentDamage = baseDamage;
        }
        else if (activeEffects.Count > 0)
        {
            // Если есть другие активные эффекты, пересчитываем их
            foreach (ActiveEffect activeEffect in activeEffects)
            {
                ApplyEffect(activeEffect.effectData);
            }
        }
        
    }
    public void AddEffect(int effectId)
    {
        EffectData effectData = effectController.GetEffectById(effectId);
        if (effectData != null && effectData.isTemporary)
        {
            // Добавляем эффект с его продолжительностью
            activeEffects.Add(new ActiveEffect(effectData, effectData.duration));
        }
        // Обработка эффекта умножения, который не временный
        if (effectData.effectName == "Multiply2")
        {
            MultiplyBalls(2);
        }
        if (effectData.effectName == "Multiply3")
        {
            MultiplyBalls(3);
        }
        if (effectData.effectName == "Multiply5")
        {
            MultiplyBalls(5);
        }
        if (effectData.effectName == "Multiply10")
        {
            MultiplyBalls(10);
        }
    }
    public void MultiplyBalls(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newBall = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
            float angle = (i == 0) ? -45 : (i == 1) ? 0 : 45;
            rb.velocity = Quaternion.Euler(0, 0, angle) * transform.GetComponent<Rigidbody2D>().velocity;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ARKANOID"))
        {
            Vector3 paddle = collision.transform.position;
            float paddleWidth = collision.collider.bounds.size.x;

            Vector3 contactPoint = collision.contacts[0].point;

            float different = contactPoint.x - paddle.x;

            float offset = different / (paddleWidth / 2);

            direct = new Vector2(offset, 1).normalized;

            transform.GetComponent<Rigidbody2D>().velocity = direct * currentSpeed;

            controller.ZeroCombo();
            ballSource.Play();
        }
        if (collision.transform.CompareTag("block"))
        {
            controller.AddCombo();
            controller.AddScore();
            try
            {
                if (activeEffects.Find(effect => effect.effectData.effectName == "BUILD").effectData.effectName == "BUILD")
                    if (collision.gameObject.GetComponent<block>().hp < 3)
                    {
                        collision.gameObject.GetComponent<block>().hp += 1;
                    }
            }
            catch
            {
                collision.gameObject.GetComponent<block>().hp -= currentDamage;
            }

            
            if (collision.gameObject.GetComponent<block>().hp <= 0)
            {
                //controller.data.BLOCKDESTROYED += 1;
                Destroy(collision.gameObject);
                controller.data.BLOCKDESTROYED += 1;
            }
            else
            {
                collision.gameObject.GetComponent<SpriteRenderer>().sprite = controller.blockHP[collision.gameObject.GetComponent<block>().hp - 1];
            }
            controller.CountBlocks();
            ballSource.Play();
            controller.data.POUNCES += 1;
            Debug.Log(controller.data.POUNCES);
            
        }
        if (collision.transform.CompareTag("WALL"))
        { 
            ballSource.Play();
            controller.data.POUNCES += 1;
            Debug.Log(controller.data.POUNCES);
        }
        if (collision.transform.name == "RestartZone")
        {
            controller.ZeroCombo();
            if (GameObject.FindGameObjectsWithTag("ball").Length == 1)
            {
                controller.hpcontroller(-1);
                controller.FindPaddleAndBall();
                return;
            }
            Destroy(gameObject);
        }
        
        /*if (collision.transform.tag == "ball")
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            Vector2 currentVelocity = transform.GetComponent<Rigidbody2D>().velocity;
            Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, collisionNormal);
            transform.GetComponent<Rigidbody2D>().velocity = reflectedVelocity;
        }*/
        


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Buff")
        {
            int effectId = collision.gameObject.GetComponent<Buff>().effectId;
            AddEffect(effectId); // Применяем эффект
            Destroy(collision.gameObject); // Уничтожаем бафф

        }
    }
}
public class ActiveEffect
{
    public EffectData effectData;
    public float remainingDuration;

    public ActiveEffect(EffectData effectData, float remainingDuration)
    {
        this.effectData = effectData;
        this.remainingDuration = remainingDuration;
    }
}