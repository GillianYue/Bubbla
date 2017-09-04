#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>

// UnityEngine.Rigidbody
struct Rigidbody_t3346577219;

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"
#include "UnityEngine_UnityEngine_Vector34282066566.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// EnemyMover
struct  EnemyMover_t3791441977  : public MonoBehaviour_t667441552
{
public:
	// System.Single EnemyMover::speed
	float ___speed_2;
	// UnityEngine.Rigidbody EnemyMover::rb
	Rigidbody_t3346577219 * ___rb_3;
	// UnityEngine.Vector3 EnemyMover::direction
	Vector3_t4282066566  ___direction_4;
	// System.Int32 EnemyMover::enemyType
	int32_t ___enemyType_5;
	// System.Int32 EnemyMover::scnRange
	int32_t ___scnRange_6;

public:
	inline static int32_t get_offset_of_speed_2() { return static_cast<int32_t>(offsetof(EnemyMover_t3791441977, ___speed_2)); }
	inline float get_speed_2() const { return ___speed_2; }
	inline float* get_address_of_speed_2() { return &___speed_2; }
	inline void set_speed_2(float value)
	{
		___speed_2 = value;
	}

	inline static int32_t get_offset_of_rb_3() { return static_cast<int32_t>(offsetof(EnemyMover_t3791441977, ___rb_3)); }
	inline Rigidbody_t3346577219 * get_rb_3() const { return ___rb_3; }
	inline Rigidbody_t3346577219 ** get_address_of_rb_3() { return &___rb_3; }
	inline void set_rb_3(Rigidbody_t3346577219 * value)
	{
		___rb_3 = value;
		Il2CppCodeGenWriteBarrier(&___rb_3, value);
	}

	inline static int32_t get_offset_of_direction_4() { return static_cast<int32_t>(offsetof(EnemyMover_t3791441977, ___direction_4)); }
	inline Vector3_t4282066566  get_direction_4() const { return ___direction_4; }
	inline Vector3_t4282066566 * get_address_of_direction_4() { return &___direction_4; }
	inline void set_direction_4(Vector3_t4282066566  value)
	{
		___direction_4 = value;
	}

	inline static int32_t get_offset_of_enemyType_5() { return static_cast<int32_t>(offsetof(EnemyMover_t3791441977, ___enemyType_5)); }
	inline int32_t get_enemyType_5() const { return ___enemyType_5; }
	inline int32_t* get_address_of_enemyType_5() { return &___enemyType_5; }
	inline void set_enemyType_5(int32_t value)
	{
		___enemyType_5 = value;
	}

	inline static int32_t get_offset_of_scnRange_6() { return static_cast<int32_t>(offsetof(EnemyMover_t3791441977, ___scnRange_6)); }
	inline int32_t get_scnRange_6() const { return ___scnRange_6; }
	inline int32_t* get_address_of_scnRange_6() { return &___scnRange_6; }
	inline void set_scnRange_6(int32_t value)
	{
		___scnRange_6 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
