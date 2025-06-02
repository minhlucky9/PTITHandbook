using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerStatsController
{
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Components")]
        public Slider slider;
        public Gradient gradient;
        public Image fill;
        public static HealthBar instance;

        [Header("Auto Drain Settings")]
        [Tooltip("Interval (in seconds) between each health tick")]
        [SerializeField] private float tickInterval = 10f;
        [Tooltip("How much health to subtract each tick")]
        [SerializeField] private float damagePerTick = 1f;

        // Lượng máu hiện tại
        private float currentHealth;

        private void Awake()
        {
            instance = this;
            // Lấy component Slider nếu chưa gán
            if (slider == null)
                slider = GetComponent<Slider>();
        }

        private void Start()
        {
            // Giả sử bạn đã gọi SetMaxHealth(...) từ script khác trước đó.
            // Nếu không, bạn có thể gọi SetMaxHealth(100f) tại đây để test.
            currentHealth = slider.maxValue;
            StartCoroutine(DrainHealthOverTime());
        }

        /// <summary>
        /// Thiết lập máu tối đa và đặt máu hiện tại = maxHealth
        /// </summary>
        public void SetMaxHealth(float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            fill.color = gradient.Evaluate(1f);
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Cập nhật lượng máu hiện tại và UI
        /// </summary>
        public void SetCurrentHealth(float newHealth)
        {
            // Giới hạn trong [0, maxValue]
            currentHealth = Mathf.Clamp(newHealth, 0f, slider.maxValue);
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);

            // Xử lý khi máu cạn
            if (currentHealth <= 0f)
            {
                // Ví dụ: phát hiện chết
                Debug.Log("Player died");
                // TODO: gọi event hoặc phương thức xử lý chết
            }
        }

        /// <summary>
        /// Coroutine tự động trừ máu sau mỗi tickInterval
        /// </summary>
        private IEnumerator DrainHealthOverTime()
        {
            // Chạy liên tục cho đến khi currentHealth <= 0
            while (currentHealth > 0f)
            {
                yield return new WaitForSeconds(tickInterval);
                SetCurrentHealth(currentHealth - damagePerTick);
            }
        }
    }
}
