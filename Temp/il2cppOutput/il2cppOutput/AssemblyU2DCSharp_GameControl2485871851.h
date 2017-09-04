#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>

// UnityEngine.GameObject
struct GameObject_t3674682005;
// UnityEngine.Camera
struct Camera_t2727095145;
// UnityEngine.GameObject[]
struct GameObjectU5BU5D_t2662109048;
// EnemySpawner
struct EnemySpawner_t2483298528;

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"
#include "UnityEngine_UnityEngine_Vector34282066566.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// GameControl
struct  GameControl_t2485871851  : public MonoBehaviour_t667441552
{
public:
	// UnityEngine.GameObject GameControl::paintBall
	GameObject_t3674682005 * ___paintBall_2;
	// UnityEngine.Camera GameControl::mainCamera
	Camera_t2727095145 * ___mainCamera_3;
	// UnityEngine.Vector3 GameControl::spawnValues
	Vector3_t4282066566  ___spawnValues_4;
	// UnityEngine.GameObject GameControl::Ballz
	GameObject_t3674682005 * ___Ballz_5;
	// UnityEngine.GameObject GameControl::Hs_Holder
	GameObject_t3674682005 * ___Hs_Holder_6;
	// System.Single GameControl::startWait
	float ___startWait_7;
	// System.Single GameControl::pbSpawnWait
	float ___pbSpawnWait_8;
	// System.Single GameControl::spawnRangeWidth
	float ___spawnRangeWidth_9;
	// System.Double GameControl::WTSfactor
	double ___WTSfactor_10;
	// System.Int32 GameControl::scWidth
	int32_t ___scWidth_11;
	// System.Int32 GameControl::scHeight
	int32_t ___scHeight_12;
	// UnityEngine.GameObject[] GameControl::hearts
	GameObjectU5BU5D_t2662109048* ___hearts_13;
	// UnityEngine.GameObject GameControl::HeartVFX
	GameObject_t3674682005 * ___HeartVFX_14;
	// UnityEngine.GameObject GameControl::player
	GameObject_t3674682005 * ___player_15;
	// UnityEngine.GameObject GameControl::GameOverC
	GameObject_t3674682005 * ___GameOverC_16;
	// EnemySpawner GameControl::eSpawner
	EnemySpawner_t2483298528 * ___eSpawner_17;

public:
	inline static int32_t get_offset_of_paintBall_2() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___paintBall_2)); }
	inline GameObject_t3674682005 * get_paintBall_2() const { return ___paintBall_2; }
	inline GameObject_t3674682005 ** get_address_of_paintBall_2() { return &___paintBall_2; }
	inline void set_paintBall_2(GameObject_t3674682005 * value)
	{
		___paintBall_2 = value;
		Il2CppCodeGenWriteBarrier(&___paintBall_2, value);
	}

	inline static int32_t get_offset_of_mainCamera_3() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___mainCamera_3)); }
	inline Camera_t2727095145 * get_mainCamera_3() const { return ___mainCamera_3; }
	inline Camera_t2727095145 ** get_address_of_mainCamera_3() { return &___mainCamera_3; }
	inline void set_mainCamera_3(Camera_t2727095145 * value)
	{
		___mainCamera_3 = value;
		Il2CppCodeGenWriteBarrier(&___mainCamera_3, value);
	}

	inline static int32_t get_offset_of_spawnValues_4() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___spawnValues_4)); }
	inline Vector3_t4282066566  get_spawnValues_4() const { return ___spawnValues_4; }
	inline Vector3_t4282066566 * get_address_of_spawnValues_4() { return &___spawnValues_4; }
	inline void set_spawnValues_4(Vector3_t4282066566  value)
	{
		___spawnValues_4 = value;
	}

	inline static int32_t get_offset_of_Ballz_5() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___Ballz_5)); }
	inline GameObject_t3674682005 * get_Ballz_5() const { return ___Ballz_5; }
	inline GameObject_t3674682005 ** get_address_of_Ballz_5() { return &___Ballz_5; }
	inline void set_Ballz_5(GameObject_t3674682005 * value)
	{
		___Ballz_5 = value;
		Il2CppCodeGenWriteBarrier(&___Ballz_5, value);
	}

	inline static int32_t get_offset_of_Hs_Holder_6() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___Hs_Holder_6)); }
	inline GameObject_t3674682005 * get_Hs_Holder_6() const { return ___Hs_Holder_6; }
	inline GameObject_t3674682005 ** get_address_of_Hs_Holder_6() { return &___Hs_Holder_6; }
	inline void set_Hs_Holder_6(GameObject_t3674682005 * value)
	{
		___Hs_Holder_6 = value;
		Il2CppCodeGenWriteBarrier(&___Hs_Holder_6, value);
	}

	inline static int32_t get_offset_of_startWait_7() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___startWait_7)); }
	inline float get_startWait_7() const { return ___startWait_7; }
	inline float* get_address_of_startWait_7() { return &___startWait_7; }
	inline void set_startWait_7(float value)
	{
		___startWait_7 = value;
	}

	inline static int32_t get_offset_of_pbSpawnWait_8() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___pbSpawnWait_8)); }
	inline float get_pbSpawnWait_8() const { return ___pbSpawnWait_8; }
	inline float* get_address_of_pbSpawnWait_8() { return &___pbSpawnWait_8; }
	inline void set_pbSpawnWait_8(float value)
	{
		___pbSpawnWait_8 = value;
	}

	inline static int32_t get_offset_of_spawnRangeWidth_9() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___spawnRangeWidth_9)); }
	inline float get_spawnRangeWidth_9() const { return ___spawnRangeWidth_9; }
	inline float* get_address_of_spawnRangeWidth_9() { return &___spawnRangeWidth_9; }
	inline void set_spawnRangeWidth_9(float value)
	{
		___spawnRangeWidth_9 = value;
	}

	inline static int32_t get_offset_of_WTSfactor_10() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___WTSfactor_10)); }
	inline double get_WTSfactor_10() const { return ___WTSfactor_10; }
	inline double* get_address_of_WTSfactor_10() { return &___WTSfactor_10; }
	inline void set_WTSfactor_10(double value)
	{
		___WTSfactor_10 = value;
	}

	inline static int32_t get_offset_of_scWidth_11() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___scWidth_11)); }
	inline int32_t get_scWidth_11() const { return ___scWidth_11; }
	inline int32_t* get_address_of_scWidth_11() { return &___scWidth_11; }
	inline void set_scWidth_11(int32_t value)
	{
		___scWidth_11 = value;
	}

	inline static int32_t get_offset_of_scHeight_12() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___scHeight_12)); }
	inline int32_t get_scHeight_12() const { return ___scHeight_12; }
	inline int32_t* get_address_of_scHeight_12() { return &___scHeight_12; }
	inline void set_scHeight_12(int32_t value)
	{
		___scHeight_12 = value;
	}

	inline static int32_t get_offset_of_hearts_13() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___hearts_13)); }
	inline GameObjectU5BU5D_t2662109048* get_hearts_13() const { return ___hearts_13; }
	inline GameObjectU5BU5D_t2662109048** get_address_of_hearts_13() { return &___hearts_13; }
	inline void set_hearts_13(GameObjectU5BU5D_t2662109048* value)
	{
		___hearts_13 = value;
		Il2CppCodeGenWriteBarrier(&___hearts_13, value);
	}

	inline static int32_t get_offset_of_HeartVFX_14() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___HeartVFX_14)); }
	inline GameObject_t3674682005 * get_HeartVFX_14() const { return ___HeartVFX_14; }
	inline GameObject_t3674682005 ** get_address_of_HeartVFX_14() { return &___HeartVFX_14; }
	inline void set_HeartVFX_14(GameObject_t3674682005 * value)
	{
		___HeartVFX_14 = value;
		Il2CppCodeGenWriteBarrier(&___HeartVFX_14, value);
	}

	inline static int32_t get_offset_of_player_15() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___player_15)); }
	inline GameObject_t3674682005 * get_player_15() const { return ___player_15; }
	inline GameObject_t3674682005 ** get_address_of_player_15() { return &___player_15; }
	inline void set_player_15(GameObject_t3674682005 * value)
	{
		___player_15 = value;
		Il2CppCodeGenWriteBarrier(&___player_15, value);
	}

	inline static int32_t get_offset_of_GameOverC_16() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___GameOverC_16)); }
	inline GameObject_t3674682005 * get_GameOverC_16() const { return ___GameOverC_16; }
	inline GameObject_t3674682005 ** get_address_of_GameOverC_16() { return &___GameOverC_16; }
	inline void set_GameOverC_16(GameObject_t3674682005 * value)
	{
		___GameOverC_16 = value;
		Il2CppCodeGenWriteBarrier(&___GameOverC_16, value);
	}

	inline static int32_t get_offset_of_eSpawner_17() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___eSpawner_17)); }
	inline EnemySpawner_t2483298528 * get_eSpawner_17() const { return ___eSpawner_17; }
	inline EnemySpawner_t2483298528 ** get_address_of_eSpawner_17() { return &___eSpawner_17; }
	inline void set_eSpawner_17(EnemySpawner_t2483298528 * value)
	{
		___eSpawner_17 = value;
		Il2CppCodeGenWriteBarrier(&___eSpawner_17, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
