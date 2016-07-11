using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroopwaffleII {
    class GameInitializer {
        // Make NetworkPed etc a subclass for it for the client so it can store actual player peds etc

        private string[] disabledScripts = {
            "am_mission_launch", "bugstar_mission_export", "fm_mission_controller", "fm_mission_creator", "sp_editor_mission_instance",
            "mission_race", "mission_repeat_controller", "mission_stat_alerter", "mission_stat_watcher", "mission_triggerer_a",
            "mission_triggerer_b", "mission_triggerer_c", "mission_triggerer_d", "net_cloud_mission_loader", "placeholdermission",
            "am_vehicle_spawn", "cablecar", "carmod_shop", "cellphone_controller", "clothes_shop_sp", "controller_ambientarea",
            "controller_races", "controller_taxi", "controller_towing", "controller_trafficking", "event_controller",
            "social_controller", "vehicle_gen_controller", "stock_controller", "stats_controller", "shop_controller",
            "pickup_controller", "friends_controller", "cheat_controller", "context_controller", "autosave_controller",
            "achievement_controller", "blip_controller", "building_controller", "candidate_controller", "pausemenu_multiplayer",
            "comms_controllers", "code_controller", "animal_controller", "friends_controllers", "restrictedareas"
        };

        public void DisableScripts() {
            foreach (string disableThisScript in disabledScripts) {
                Game.TerminateAllScriptsWithName(disableThisScript);
            }
        }

        public void DisableByFrame() {
            NativeFunction.Natives.SetVehicleDensityMultiplierThisFrame(0f);
            NativeFunction.Natives.SetRandomVehicleDensityMultiplierThisFrame(0f);
            NativeFunction.Natives.SetParkedVehicleDensityMultiplierThisFrame(0f);
            NativeFunction.Natives.SetPedDensityMultiplierThisFrame(0f);
            NativeFunction.Natives.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
            NativeFunction.Natives.DisableControlAction(0, 19, 1);
            NativeFunction.Natives.DisableControlAction(0, 44, 1);
            NativeFunction.Natives.DisableControlAction(0, 171, 1);
            NativeFunction.Natives.SetMaxWantedLevel(0);
            NativeFunction.Natives.SetGarbageTrucks(0);
            NativeFunction.Natives.SetRandomBoats(0);
            NativeFunction.Natives.SetRandomTrains(0);
        }

        public void RemoveAllEntities() {
            // remove all vehicles from the game world
            if (World.EnumerateVehicles().Count() > 0) {
                World.GetAllVehicles().ToList().ForEach(entity => entity.Delete());
            }
        }
    }
}
