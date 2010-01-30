using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace PhysicsGame
{
    public class ModSound
    {
        const int channels = 8;
        Dictionary<String, SoundEffect> sounds;
        SoundEffectInstance[] sound = new SoundEffectInstance[channels];
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();
        Vector3 position = Vector3.Zero;
        int current = 0;

        public ModSound()
        {
            sounds = new Dictionary<string, SoundEffect>();
        }

        public void addSound(String name, SoundEffect snd)
        {
            sounds.Add(name, snd);
        }

        public void playSound (String snd, Vector2 src) 
        {
            Vector3 a = new Vector3(src.X, src.Y, 0);
            emitter.Position = a;
            Vector3 temp = a - position;
            temp *= 1;
            listener.Position = a + temp;
            if (sound[current] != null && sound[current].State.Equals(SoundState.Playing)) sound[current].Stop();
            sound[current] = sounds[snd].Play3D(listener, emitter, 1f, 0, false);
            current++;
            if (current >= channels) current = 0;
        }

        public void stopAll()
        {
            for (int i = 0; i < channels; i++)
                if (sound[i] != null && sound[i].State.Equals(SoundState.Playing)) sound[i].Stop();
        }
    }
}
