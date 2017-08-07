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
	// System.Single GameControl::startWait
	float ___startWait_6;
	// System.Single GameControl::pbSpawnWait
	float ___pbSpawnWait_7;
	// System.Single GameControl::spawnRangeWidth
	float ___spawnRangeWidth_8;
	// System.Double GameControl::WTSfactor
	double ___WTSfactor_9;
	// System.Int32 GameControl::scWidth
	int32_t ___scWidth_10;
	// System.Int32 GameControl::scHeight
	int32_t ___scHeight_11;
	// UnityEngine.GameObject GameControl::player
	GameObject_t3674682005 * ___player_12;

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

	inline static int32_t get_offset_of_startWait_6() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___startWait_6)); }
	inline float get_startWait_6() const { return ___startWait_6; }
	inline float* get_address_of_startWait_6() { return &___startWait_6; }
	inline void set_startWait_6(float value)
	{
		___startWait_6 = value;
	}

	inline static int32_t get_offset_of_pbSpawnWait_7() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___pbSpawnWait_7)); }
	inline float get_pbSpawnWait_7() const { return ___pbSpawnWait_7; }
	inline float* get_address_of_pbSpawnWait_7() { return &___pbSpawnWait_7; }
	inline void set_pbSpawnWait_7(float value)
	{
		___pbSpawnWait_7 = value;
	}

	inline static int32_t get_offset_of_spawnRangeWidth_8() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___spawnRangeWidth_8)); }
	inline float get_spawnRangeWidth_8() const { return ___spawnRangeWidth_8; }
	inline float* get_address_of_spawnRangeWidth_8() { return &___spawnRangeWidth_8; }
	inline void set_spawnRangeWidth_8(float value)
	{
		___spawnRangeWidth_8 = value;
	}

	inline static int32_t get_offset_of_WTSfactor_9() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___WTSfactor_9)); }
	inline double get_WTSfactor_9() const { return ___WTSfactor_9; }
	inline double* get_address_of_WTSfactor_9() { return &___WTSfactor_9; }
	inline void set_WTSfactor_9(double value)
	{
		___WTSfactor_9 = value;
	}

	inline static int32_t get_offset_of_scWidth_10() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___scWidth_10)); }
	inline int32_t get_scWidth_10() const { return ___scWidth_10; }
	inline int32_t* get_address_of_scWidth_10() { return &___scWidth_10; }
	inline void set_scWidth_10(int32_t value)
	{
		___scWidth_10 = value;
	}

	inline static int32_t get_offset_of_scHeight_11() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___scHeight_11)); }
	inline int32_t get_scHeight_11() const { return ___scHeight_11; }
	inline int32_t* get_address_of_scHeight_11() { return &___scHeight_11; }
	inline void set_scHeight_11(int32_t value)
	{
		___scHeight_11 = value;
	}

	inline static int32_t get_offset_of_player_12() { return static_cast<int32_t>(offsetof(GameControl_t2485871851, ___player_12)); }
	inline GameObject_t3674682005 * get_player_12() const { return ___player_12; }
	inline GameObject_t3674682005 ** get_address_of_player_12() { return &___player_12; }
	inline void set_player_12(GameObject_t3674682005 * value)
	{
		___player_12 = value;
		Il2CppCodeGenWriteBarrier(&___player_12, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
