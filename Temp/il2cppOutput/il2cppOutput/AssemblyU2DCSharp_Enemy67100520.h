#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>


#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// Enemy
struct  Enemy_t67100520  : public MonoBehaviour_t667441552
{
public:
	// System.Int32 Enemy::life
	int32_t ___life_2;
	// System.Int32 Enemy::attack
	int32_t ___attack_3;

public:
	inline static int32_t get_offset_of_life_2() { return static_cast<int32_t>(offsetof(Enemy_t67100520, ___life_2)); }
	inline int32_t get_life_2() const { return ___life_2; }
	inline int32_t* get_address_of_life_2() { return &___life_2; }
	inline void set_life_2(int32_t value)
	{
		___life_2 = value;
	}

	inline static int32_t get_offset_of_attack_3() { return static_cast<int32_t>(offsetof(Enemy_t67100520, ___attack_3)); }
	inline int32_t get_attack_3() const { return ___attack_3; }
	inline int32_t* get_address_of_attack_3() { return &___attack_3; }
	inline void set_attack_3(int32_t value)
	{
		___attack_3 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
