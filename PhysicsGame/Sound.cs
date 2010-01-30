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
        const int channels = 1;
        
        Dictionary<String, SoundEffect> sounds;
        Dictionary<Object, SoundEffectInstance> soundInstances;

        SoundEffectInstance[] sound = new SoundEffectInstance[channels];
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();
        Vector3 position = Vector3.Zero;

        public ModSound()
        {
            sounds = new Dictionary<string, SoundEffect>();
            soundInstances = new Dictionary<object, SoundEffectInstance>();
        }

        public void addSound(String name, SoundEffect snd)
        {
            sounds[name] = snd;
        }

        public void playSound (String snd, Object obj, Vector2 src, float vol) 
        {
            if (!soundInstances.ContainsKey(obj) || soundInstances[obj].State.Equals(SoundState.Stopped))
                soundInstances[obj] = sounds[snd].Play3D(listener, emitter, vol, 0, false);
        }

        public void stopAll()
        {
            for (int i = 0; i < channels; i++)
                if (sound[i] != null && sound[i].State.Equals(SoundState.Playing)) sound[i].Stop();
        }
    }
}
