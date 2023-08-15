using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class HapticManager : MonoBehaviour
    {
        public virtual void SelectionBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        }

        public virtual void SuccessBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }

        public virtual void WarningBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        }

        public virtual void FailureBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);
        }

        public virtual void RigidBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        }

        public virtual void SoftBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        }

        public virtual void LightBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }

        public virtual void MediumBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        }

        public virtual void HeavyBoop()
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
        }
    }
}
