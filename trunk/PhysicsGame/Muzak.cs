using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace PhysicsGame
{
    class Muzak
    {
        const int channels = 1;
        
        Dictionary<String, SoundEffect> sounds;
        SoundEffectInstance MenuMusic;
        SoundEffectInstance GameMusic;
        String current;
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();

        enum ok
        {
            menuState,
            gameState
        }

        ok state = ok.menuState;

        public Muzak ()
        {
            sounds = new Dictionary<string, SoundEffect>();
        }

        public void addSound(String name, SoundEffect snd)
        {
            sounds[name] = snd;
        }

        public void startMenu(String k)
        {
            if (GameMusic != null) GameMusic.Stop();
            current = k;
            MenuMusic = sounds[current].Play3D(listener, emitter, 0.35f, 0f, true);
            state = ok.menuState;
        }

        public void startLevel(String k)
        {
            if (MenuMusic != null) MenuMusic.Stop();
            current = k;
            GameMusic = sounds[current].Play3D(listener, emitter, 0.35f, 0f, false);
            state = ok.gameState;
        }

        public void swapMuzak()
        {
            if (state == ok.menuState)
            {
                MenuMusic.Pause();
                GameMusic.Resume();
                state = ok.gameState;
            }
            else
            {
                GameMusic.Pause();
                MenuMusic.Resume();
                state = ok.menuState;
            }
        }

        public void stopAll()
        {
            GameMusic.Stop();
            MenuMusic.Stop();
        }
    }
}
