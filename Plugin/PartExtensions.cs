/* Copyright © 2013-2015, Elián Hanisch <lambdae2@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;

namespace RCSBuildAid
{
    public static class PartExtensions
    {
        public static bool Physicsless (this Part part)
        {
            if (part == EditorLogic.RootPart) {
                return false;
            }
            if (part.physicalSignificance == Part.PhysicalSignificance.NONE) {
                return true;
            }
            return false;
        }

        public static bool GroundParts (this Part part)
        {
            /* Ground parts, stuff that stays in the ground at launch */
            return part.Modules.Contains ("LaunchClamp");
        }

        public static float GetTotalMass (this Part part) {
            var mass = part.partInfo.partPrefab.mass;
            return mass + part.GetModuleMass (mass) + part.GetResourceMass ();
        }

        public static float GetDryMass (this Part part) {
            var mass = part.partInfo.partPrefab.mass;
            return mass + part.GetModuleMass (mass);
        }

        public static float GetPhysicslessChildMassInEditor (this Part part) {
            /* in the editor parts aren't setup for part.GetPhysicsLessChildMass () to work. */
            float m = 0f;
            for (int i = 0; i < part.children.Count; i++) {
                Part child = part.children [i];
                if (child.Physicsless ()) {
                    m += child.GetTotalMass ();
                }
            }
            return m;
        }

        static Vector3 getCoM (Part part) {
            /* part.WCoM fails in the editor */
            return part.partTransform.position + part.partTransform.rotation * part.CoMOffset;
        }

        public static bool GetCoM (this Part part, out Vector3 com)
        {
            if (part.Physicsless ()) {
                /* the only part that has no parent is the root, which always has physics.
                 * selected parts only get here when they have a potential parent */
                Part parent = part.parent ?? part.potentialParent;
                /* it seems that a physicsless part attached to another
                 * physicsless part won't have its mass accounted */
                if ((parent == null) || parent.Physicsless ()) {
                    com = Vector3.zero;
                    return false;
                } else {
                    com = getCoM(parent);
                }
            } else {
                com = getCoM(part);
            }
            return true;
        }
    }
}

