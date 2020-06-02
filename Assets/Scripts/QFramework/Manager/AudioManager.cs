using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AudioManager : MonoBehaviour
    {
  
        private static AudioManager mInstance;

        public static AudioManager Instance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = new GameObject("AudioManager").AddComponent<AudioManager>();

                    DontDestroyOnLoad(mInstance);
                }

                return mInstance;
            }
        }

        /// <summary>
        /// 背景音乐是否静音
        /// </summary>
        private bool isMusicMute = false;

        public bool MusicMute()
        {
            return isMusicMute;
        }

        /// <summary>
        /// 音效是否关闭
        /// </summary>
        private bool isSoundMute = false;

        public bool SoundMute()
        {
            return isSoundMute;
        }

        /// <summary>
        /// 音乐监听
        /// </summary>
        private AudioListener mAudioListener;
        /// <summary>
        /// 当前背景音乐的source
        /// </summary>
        private AudioSource mMusicSource;

        private Dictionary<string, AudioSource> SoundSource = new Dictionary<string, AudioSource>();

        private Dictionary<string, AudioSource> MusicSource = new Dictionary<string, AudioSource>();

        /// <summary>
        /// 是否存在音乐监听
        /// </summary>
        private void CheckAudioListener()
        {
            if (!mAudioListener)
            {
                mAudioListener = gameObject.AddComponent<AudioListener>();
            }
        }

        /// <summary>
        /// 创建相应的背景音乐
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="loop"></param>
        private void CreateMusicAudio(string soundName, bool loop)
        {
            if (mMusicSource)
            {
                mMusicSource.Stop();
            }
            mMusicSource = gameObject.AddComponent<AudioSource>();
            var ResourcesSound = Resources.Load<AudioClip>(soundName);
            mMusicSource.clip = ResourcesSound;
            mMusicSource.loop = loop;
            MusicSource.Add(soundName, mMusicSource);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="soundName">音乐名</param>
        /// <param name="loop">是否循环</param>
        public void PlayMusic(string soundName, bool loop)
        {
            CheckAudioListener();

            bool changeSource = false;

            if (mMusicSource)
            {
                if(mMusicSource.clip.name != soundName)
                {
                    changeSource = true;
                }
            }
            else
            {
                changeSource = true;
            }

            if (changeSource)
            {
                if (MusicSource.ContainsKey(soundName))
                {
                    mMusicSource.Stop();
                    mMusicSource = MusicSource[soundName];
                }
                else
                {
                    CreateMusicAudio(soundName, loop);
                }
            }
            if (!isMusicMute && !mMusicSource.isPlaying)
            {
                mMusicSource.Play();
            }
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音乐名</param>
        public void PlaySound(string soundName)
        {
            CheckAudioListener();

            AudioSource audioSource;

            if(SoundSource.ContainsKey(soundName))
            {
                audioSource = SoundSource[soundName];
            }
            else
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                var ResourcesSound = Resources.Load<AudioClip>(soundName);
                audioSource.clip = ResourcesSound;
                SoundSource.Add(soundName, audioSource);
            }
           
            if(!isSoundMute)
            {
                audioSource.Play();
            }
        }
        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (mMusicSource)
            {
                mMusicSource.Stop();
            }
        }
        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseMusic()
        {
            if (mMusicSource)
            {
                mMusicSource.Pause();
            }
        }
        /// <summary>
        /// 回复背景音乐
        /// </summary>
        public void ResumeMusic()
        {
            if (mMusicSource)
            {
                mMusicSource.UnPause();
            }
        }
        /// <summary>
        /// 禁止背景音乐
        /// </summary>
        public void MusicOff()
        {
            isMusicMute = true;
            if (mMusicSource != null)
            {
                mMusicSource.Pause();
                mMusicSource.mute = true;
            }
        }
        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public void MusicOn()
        {
            isMusicMute = false;
            if (mMusicSource != null)
            {
                mMusicSource.UnPause();
                mMusicSource.mute = false;
            }
        }
        /// <summary>
        /// 禁止音效
        /// </summary>
        public void SoundOff()
        {
            isSoundMute = true;
            foreach(KeyValuePair<string, AudioSource> source in SoundSource)
            {
                AudioSource audioSource = source.Value;
                audioSource.Pause();
                audioSource.mute = true;
            }
        }
        /// <summary>
        /// 恢复音效
        /// </summary>
        public void SoundOn()
        {
            isSoundMute = false;
            foreach (KeyValuePair<string, AudioSource> source in SoundSource)
            {
                AudioSource audioSource = source.Value;
                audioSource.UnPause();
                audioSource.mute = false;
            }
        }

    }
}

