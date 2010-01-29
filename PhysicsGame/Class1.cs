using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace PhysicsGame
{
    class Sound
    {
        //SoundEffect gunShot;
        Dictionary<String, SoundEffect> sounds;
        SoundEffectInstance sound;
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();
        Vector3 position = Vector3.Zero;

        void addSound(SoundEffect snd)
        {
            //sounds.Add(snd);
        }

        void playSound (String snd, Vector2 src) {
            Vector3 a = new Vector3(src.X, src.Y, 0);
            emitter.Position = a;
            Vector3 temp = a - position;
            temp *= 1;
            listener.Position = a + temp;
            sound = sounds[snd].Play3D(listener, emitter, 1f, 0, false);
        }
    }
}
