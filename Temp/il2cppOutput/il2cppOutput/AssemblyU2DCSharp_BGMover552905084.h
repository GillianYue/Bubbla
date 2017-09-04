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

// BGMover
struct  BGMover_t552905084  : public MonoBehaviour_t667441552
{
public:
	// System.Single BGMover::topZ
	float ___topZ_2;
	// System.Single BGMover::bottomZ
	float ___bottomZ_3;
	// System.Single BGMover::startZ
	float ___startZ_4;
	// System.Single BGMover::scrollSpd
	float ___scrollSpd_5;
	// System.Boolean BGMover::scrollin
	bool ___scrollin_6;

public:
	inline static int32_t get_offset_of_topZ_2() { return static_cast<int32_t>(offsetof(BGMover_t552905084, ___topZ_2)); }
	inline float get_topZ_2() const { return ___topZ_2; }
	inline float* get_address_of_topZ_2() { return &___topZ_2; }
	inline void set_topZ_2(float value)
	{
		___topZ_2 = value;
	}

	inline static int32_t get_offset_of_bottomZ_3() { return static_cast<int32_t>(offsetof(BGMover_t552905084, ___bottomZ_3)); }
	inline float get_bottomZ_3() const { return ___bottomZ_3; }
	inline float* get_address_of_bottomZ_3() { return &___bottomZ_3; }
	inline void set_bottomZ_3(float value)
	{
		___bottomZ_3 = value;
	}

	inline static int32_t get_offset_of_startZ_4() { return static_cast<int32_t>(offsetof(BGMover_t552905084, ___startZ_4)); }
	inline float get_startZ_4() const { return ___startZ_4; }
	inline float* get_address_of_startZ_4() { return &___startZ_4; }
	inline void set_startZ_4(float value)
	{
		___startZ_4 = value;
	}

	inline static int32_t get_offset_of_scrollSpd_5() { return static_cast<int32_t>(offsetof(BGMover_t552905084, ___scrollSpd_5)); }
	inline float get_scrollSpd_5() const { return ___scrollSpd_5; }
	inline float* get_address_of_scrollSpd_5() { return &___scrollSpd_5; }
	inline void set_scrollSpd_5(float value)
	{
		___scrollSpd_5 = value;
	}

	inline static int32_t get_offset_of_scrollin_6() { return static_cast<int32_t>(offsetof(BGMover_t552905084, ___scrollin_6)); }
	inline bool get_scrollin_6() const { return ___scrollin_6; }
	inline bool* get_address_of_scrollin_6() { return &___scrollin_6; }
	inline void set_scrollin_6(bool value)
	{
		___scrollin_6 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
