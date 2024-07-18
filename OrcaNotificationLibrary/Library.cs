using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrcaNotificationLibrary
{
    // this only works on pc visual, its an ongui :sob:
    public class Library : MonoBehaviour
    {
        public class SomewhatRobloxAnimationRecreations
        {
            public static float QuintOut(float t, float b, float c, float d)
            {
                t /= d;
                t--;
                return c * (t * t * t * t * t + 1) + b;
            }   // this is supposed to be quint out, pretty smooth imo
        }

        public class Notification
        {
            public string message;
            public Rect rect;
            public float targetX;
            public bool isEntering;
            public bool isExiting;
            public float duration;
            public float elapsedTime;
            public Texture2D texture;
            public Color backgroundColor;

            public Notification(string message, Rect rect, float duration, Color backgroundColor)
            {
                this.message = message;
                this.rect = rect;
                this.duration = duration;
                this.elapsedTime = 0f;
                this.isEntering = true;
                this.isExiting = false;
                this.targetX = rect.x;
                this.rect.x = -rect.width;
                this.backgroundColor = backgroundColor;

                texture = new Texture2D(2, 2);
                for (int i = 0; i < texture.width; i++)
                {
                    for (int j = 0; j < texture.height; j++)
                    {
                        texture.SetPixel(i, j, Color.white);
                    }
                }
                texture.Apply();
            }
        }

        private List<Notification> Notifications = new List<Notification>();
        public GUIStyle NotiStyle;

        void OnGUI()
        {
            for (int i = 0; i < Notifications.Count; i++)
            {
                Notification notification = Notifications[i];
                notification.rect.y = Mathf.Lerp(notification.rect.y, 10f + i * 40f, Time.deltaTime * 10f);

                Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(notification.message));

                notification.rect.width = labelSize.x + 20f;
                notification.rect.height = labelSize.y + 10f;

                Draw(notification.rect, notification.backgroundColor, 8f, notification.texture);
                GUI.Label(new Rect(notification.rect.x + 10f, notification.rect.y + 5f, labelSize.x, labelSize.y), notification.message);
            }
        }

        public void NewNotification(string message, Color backgroundColor, float duration = 2f)
        {
            Notification newNotification = new Notification(message, new Rect(10f, 10f, 350f, 30f), duration, backgroundColor);
            Notifications.Add(newNotification);
            StartCoroutine(AnimateNotification(newNotification));
        }

        private IEnumerator AnimateNotification(Notification notification)
        {
            while (notification.isEntering)
            {
                notification.elapsedTime += Time.deltaTime;
                notification.rect.x = SomewhatRobloxAnimationRecreations.QuintOut(notification.elapsedTime, -notification.rect.width, notification.targetX + notification.rect.width, 0.5f);
                if (notification.elapsedTime >= 0.5f)
                {
                    notification.isEntering = false;
                    notification.elapsedTime = 0f;
                    StartCoroutine(ExitNotification(notification));
                    yield break;
                }
                yield return null;
            }
        }

        private IEnumerator ExitNotification(Notification notification)
        {
            yield return new WaitForSeconds(notification.duration);
            notification.isExiting = true;
            notification.elapsedTime = 0f;
            float initialX = notification.rect.x;
            while (notification.isExiting)
            {
                notification.elapsedTime += Time.deltaTime;
                notification.rect.x = SomewhatRobloxAnimationRecreations.QuintOut(notification.elapsedTime, initialX, -notification.rect.width - initialX, 0.5f);
                if (notification.elapsedTime >= 0.5f)
                {
                    Notifications.Remove(notification);
                    yield break;
                }
                yield return null;
            }
        }

        void Draw(Rect size, Color color, float r, Texture2D texture)
        {
            GUI.DrawTexture(
                size,
                texture,
                (ScaleMode)(0 + Type.EmptyTypes.Length),
                Type.EmptyTypes.Length != 0,
                0f,
                color,
                0f,
                r
            );
        }
    }
}
