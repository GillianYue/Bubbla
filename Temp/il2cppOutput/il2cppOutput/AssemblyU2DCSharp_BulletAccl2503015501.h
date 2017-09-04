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

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// BulletAccl
struct  BulletAccl_t2503015501  : public MonoBehaviour_t667441552
{
public:
	// System.Single BulletAccl::accl
	float ___accl_2;
	// UnityEngine.GameObject BulletAccl::trail
	GameObject_t3674682005 * ___trail_3;
	// UnityEngine.GameObject BulletAccl::explosion
	GameObject_t3674682005 * ___explosion_4;

public:
	inline static int32_t get_offset_of_accl_2() { return static_cast<int32_t>(offsetof(BulletAccl_t2503015501, ___accl_2)); }
	inline float get_accl_2() const { return ___accl_2; }
	inline float* get_address_of_accl_2() { return &___accl_2; }
	inline void set_accl_2(float value)
	{
		___accl_2 = value;
	}

	inline static int32_t get_offset_of_trail_3() { return static_cast<int32_t>(offsetof(BulletAccl_t2503015501, ___trail_3)); }
	inline GameObject_t3674682005 * get_trail_3() const { return ___trail_3; }
	inline GameObject_t3674682005 ** get_address_of_trail_3() { return &___trail_3; }
	inline void set_trail_3(GameObject_t3674682005 * value)
	{
		___trail_3 = value;
		Il2CppCodeGenWriteBarrier(&___trail_3, value);
	}

	inline static int32_t get_offset_of_explosion_4() { return static_cast<int32_t>(offsetof(BulletAccl_t2503015501, ___explosion_4)); }
	inline GameObject_t3674682005 * get_explosion_4() const { return ___explosion_4; }
	inline GameObject_t3674682005 ** get_address_of_explosion_4() { return &___explosion_4; }
	inline void set_explosion_4(GameObject_t3674682005 * value)
	{
		___explosion_4 = value;
		Il2CppCodeGenWriteBarrier(&___explosion_4, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
