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
// DestroyByTime
struct DestroyByTime_t2168858686;
// UnityEngine.ParticleSystem
struct ParticleSystem_t381473177;

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// TrailFollowBall
struct  TrailFollowBall_t1723313110  : public MonoBehaviour_t667441552
{
public:
	// UnityEngine.GameObject TrailFollowBall::myBullet
	GameObject_t3674682005 * ___myBullet_2;
	// System.Single TrailFollowBall::lastingTime
	float ___lastingTime_3;
	// DestroyByTime TrailFollowBall::dbt
	DestroyByTime_t2168858686 * ___dbt_4;
	// UnityEngine.ParticleSystem TrailFollowBall::smokeTrail
	ParticleSystem_t381473177 * ___smokeTrail_5;

public:
	inline static int32_t get_offset_of_myBullet_2() { return static_cast<int32_t>(offsetof(TrailFollowBall_t1723313110, ___myBullet_2)); }
	inline GameObject_t3674682005 * get_myBullet_2() const { return ___myBullet_2; }
	inline GameObject_t3674682005 ** get_address_of_myBullet_2() { return &___myBullet_2; }
	inline void set_myBullet_2(GameObject_t3674682005 * value)
	{
		___myBullet_2 = value;
		Il2CppCodeGenWriteBarrier(&___myBullet_2, value);
	}

	inline static int32_t get_offset_of_lastingTime_3() { return static_cast<int32_t>(offsetof(TrailFollowBall_t1723313110, ___lastingTime_3)); }
	inline float get_lastingTime_3() const { return ___lastingTime_3; }
	inline float* get_address_of_lastingTime_3() { return &___lastingTime_3; }
	inline void set_lastingTime_3(float value)
	{
		___lastingTime_3 = value;
	}

	inline static int32_t get_offset_of_dbt_4() { return static_cast<int32_t>(offsetof(TrailFollowBall_t1723313110, ___dbt_4)); }
	inline DestroyByTime_t2168858686 * get_dbt_4() const { return ___dbt_4; }
	inline DestroyByTime_t2168858686 ** get_address_of_dbt_4() { return &___dbt_4; }
	inline void set_dbt_4(DestroyByTime_t2168858686 * value)
	{
		___dbt_4 = value;
		Il2CppCodeGenWriteBarrier(&___dbt_4, value);
	}

	inline static int32_t get_offset_of_smokeTrail_5() { return static_cast<int32_t>(offsetof(TrailFollowBall_t1723313110, ___smokeTrail_5)); }
	inline ParticleSystem_t381473177 * get_smokeTrail_5() const { return ___smokeTrail_5; }
	inline ParticleSystem_t381473177 ** get_address_of_smokeTrail_5() { return &___smokeTrail_5; }
	inline void set_smokeTrail_5(ParticleSystem_t381473177 * value)
	{
		___smokeTrail_5 = value;
		Il2CppCodeGenWriteBarrier(&___smokeTrail_5, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
