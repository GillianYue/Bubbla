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

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"
#include "UnityEngine_UnityEngine_Vector34282066566.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// EnemySpawner
struct  EnemySpawner_t2483298528  : public MonoBehaviour_t667441552
{
public:
	// System.Single EnemySpawner::startWait
	float ___startWait_2;
	// System.Single EnemySpawner::enemySpawnWait
	float ___enemySpawnWait_3;
	// System.Single EnemySpawner::waveSpawnWait
	float ___waveSpawnWait_4;
	// System.Single EnemySpawner::range
	float ___range_5;
	// UnityEngine.GameObject EnemySpawner::spike
	GameObject_t3674682005 * ___spike_6;
	// UnityEngine.GameObject EnemySpawner::enemiz
	GameObject_t3674682005 * ___enemiz_7;
	// UnityEngine.GameObject EnemySpawner::bird
	GameObject_t3674682005 * ___bird_8;
	// UnityEngine.Vector3 EnemySpawner::spawnValues
	Vector3_t4282066566  ___spawnValues_9;

public:
	inline static int32_t get_offset_of_startWait_2() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___startWait_2)); }
	inline float get_startWait_2() const { return ___startWait_2; }
	inline float* get_address_of_startWait_2() { return &___startWait_2; }
	inline void set_startWait_2(float value)
	{
		___startWait_2 = value;
	}

	inline static int32_t get_offset_of_enemySpawnWait_3() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___enemySpawnWait_3)); }
	inline float get_enemySpawnWait_3() const { return ___enemySpawnWait_3; }
	inline float* get_address_of_enemySpawnWait_3() { return &___enemySpawnWait_3; }
	inline void set_enemySpawnWait_3(float value)
	{
		___enemySpawnWait_3 = value;
	}

	inline static int32_t get_offset_of_waveSpawnWait_4() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___waveSpawnWait_4)); }
	inline float get_waveSpawnWait_4() const { return ___waveSpawnWait_4; }
	inline float* get_address_of_waveSpawnWait_4() { return &___waveSpawnWait_4; }
	inline void set_waveSpawnWait_4(float value)
	{
		___waveSpawnWait_4 = value;
	}

	inline static int32_t get_offset_of_range_5() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___range_5)); }
	inline float get_range_5() const { return ___range_5; }
	inline float* get_address_of_range_5() { return &___range_5; }
	inline void set_range_5(float value)
	{
		___range_5 = value;
	}

	inline static int32_t get_offset_of_spike_6() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___spike_6)); }
	inline GameObject_t3674682005 * get_spike_6() const { return ___spike_6; }
	inline GameObject_t3674682005 ** get_address_of_spike_6() { return &___spike_6; }
	inline void set_spike_6(GameObject_t3674682005 * value)
	{
		___spike_6 = value;
		Il2CppCodeGenWriteBarrier(&___spike_6, value);
	}

	inline static int32_t get_offset_of_enemiz_7() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___enemiz_7)); }
	inline GameObject_t3674682005 * get_enemiz_7() const { return ___enemiz_7; }
	inline GameObject_t3674682005 ** get_address_of_enemiz_7() { return &___enemiz_7; }
	inline void set_enemiz_7(GameObject_t3674682005 * value)
	{
		___enemiz_7 = value;
		Il2CppCodeGenWriteBarrier(&___enemiz_7, value);
	}

	inline static int32_t get_offset_of_bird_8() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___bird_8)); }
	inline GameObject_t3674682005 * get_bird_8() const { return ___bird_8; }
	inline GameObject_t3674682005 ** get_address_of_bird_8() { return &___bird_8; }
	inline void set_bird_8(GameObject_t3674682005 * value)
	{
		___bird_8 = value;
		Il2CppCodeGenWriteBarrier(&___bird_8, value);
	}

	inline static int32_t get_offset_of_spawnValues_9() { return static_cast<int32_t>(offsetof(EnemySpawner_t2483298528, ___spawnValues_9)); }
	inline Vector3_t4282066566  get_spawnValues_9() const { return ___spawnValues_9; }
	inline Vector3_t4282066566 * get_address_of_spawnValues_9() { return &___spawnValues_9; }
	inline void set_spawnValues_9(Vector3_t4282066566  value)
	{
		___spawnValues_9 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
