#pragma once

#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif

#include <stdint.h>

// System.Collections.Generic.List`1<UnityEngine.Color>
struct List_1_t1267765161;
// UnityEngine.UI.Text
struct Text_t9039225;
// UnityEngine.GameObject
struct GameObject_t3674682005;
// System.Collections.Generic.List`1<UnityEngine.GameObject>
struct List_1_t747900261;
// GameControl
struct GameControl_t2485871851;

#include "UnityEngine_UnityEngine_MonoBehaviour667441552.h"

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// Player
struct  Player_t2393081601  : public MonoBehaviour_t667441552
{
public:
	// UnityEngine.UI.Text Player::lifeText
	Text_t9039225 * ___lifeText_3;
	// UnityEngine.GameObject Player::PaintSpriteObj
	GameObject_t3674682005 * ___PaintSpriteObj_4;
	// UnityEngine.GameObject Player::BulletGaugeObj
	GameObject_t3674682005 * ___BulletGaugeObj_5;
	// UnityEngine.GameObject Player::BulletObj
	GameObject_t3674682005 * ___BulletObj_6;
	// UnityEngine.GameObject Player::cannon
	GameObject_t3674682005 * ___cannon_7;
	// System.Collections.Generic.List`1<UnityEngine.GameObject> Player::PaintSprites
	List_1_t747900261 * ___PaintSprites_8;
	// System.Int32 Player::bulletGaugeCapacity
	int32_t ___bulletGaugeCapacity_9;
	// System.Int32 Player::maxLife
	int32_t ___maxLife_10;
	// System.Single Player::bulletSpeed
	float ___bulletSpeed_11;
	// System.Int32 Player::life
	int32_t ___life_12;
	// GameControl Player::gameControl
	GameControl_t2485871851 * ___gameControl_13;

public:
	inline static int32_t get_offset_of_lifeText_3() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___lifeText_3)); }
	inline Text_t9039225 * get_lifeText_3() const { return ___lifeText_3; }
	inline Text_t9039225 ** get_address_of_lifeText_3() { return &___lifeText_3; }
	inline void set_lifeText_3(Text_t9039225 * value)
	{
		___lifeText_3 = value;
		Il2CppCodeGenWriteBarrier(&___lifeText_3, value);
	}

	inline static int32_t get_offset_of_PaintSpriteObj_4() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___PaintSpriteObj_4)); }
	inline GameObject_t3674682005 * get_PaintSpriteObj_4() const { return ___PaintSpriteObj_4; }
	inline GameObject_t3674682005 ** get_address_of_PaintSpriteObj_4() { return &___PaintSpriteObj_4; }
	inline void set_PaintSpriteObj_4(GameObject_t3674682005 * value)
	{
		___PaintSpriteObj_4 = value;
		Il2CppCodeGenWriteBarrier(&___PaintSpriteObj_4, value);
	}

	inline static int32_t get_offset_of_BulletGaugeObj_5() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___BulletGaugeObj_5)); }
	inline GameObject_t3674682005 * get_BulletGaugeObj_5() const { return ___BulletGaugeObj_5; }
	inline GameObject_t3674682005 ** get_address_of_BulletGaugeObj_5() { return &___BulletGaugeObj_5; }
	inline void set_BulletGaugeObj_5(GameObject_t3674682005 * value)
	{
		___BulletGaugeObj_5 = value;
		Il2CppCodeGenWriteBarrier(&___BulletGaugeObj_5, value);
	}

	inline static int32_t get_offset_of_BulletObj_6() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___BulletObj_6)); }
	inline GameObject_t3674682005 * get_BulletObj_6() const { return ___BulletObj_6; }
	inline GameObject_t3674682005 ** get_address_of_BulletObj_6() { return &___BulletObj_6; }
	inline void set_BulletObj_6(GameObject_t3674682005 * value)
	{
		___BulletObj_6 = value;
		Il2CppCodeGenWriteBarrier(&___BulletObj_6, value);
	}

	inline static int32_t get_offset_of_cannon_7() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___cannon_7)); }
	inline GameObject_t3674682005 * get_cannon_7() const { return ___cannon_7; }
	inline GameObject_t3674682005 ** get_address_of_cannon_7() { return &___cannon_7; }
	inline void set_cannon_7(GameObject_t3674682005 * value)
	{
		___cannon_7 = value;
		Il2CppCodeGenWriteBarrier(&___cannon_7, value);
	}

	inline static int32_t get_offset_of_PaintSprites_8() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___PaintSprites_8)); }
	inline List_1_t747900261 * get_PaintSprites_8() const { return ___PaintSprites_8; }
	inline List_1_t747900261 ** get_address_of_PaintSprites_8() { return &___PaintSprites_8; }
	inline void set_PaintSprites_8(List_1_t747900261 * value)
	{
		___PaintSprites_8 = value;
		Il2CppCodeGenWriteBarrier(&___PaintSprites_8, value);
	}

	inline static int32_t get_offset_of_bulletGaugeCapacity_9() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___bulletGaugeCapacity_9)); }
	inline int32_t get_bulletGaugeCapacity_9() const { return ___bulletGaugeCapacity_9; }
	inline int32_t* get_address_of_bulletGaugeCapacity_9() { return &___bulletGaugeCapacity_9; }
	inline void set_bulletGaugeCapacity_9(int32_t value)
	{
		___bulletGaugeCapacity_9 = value;
	}

	inline static int32_t get_offset_of_maxLife_10() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___maxLife_10)); }
	inline int32_t get_maxLife_10() const { return ___maxLife_10; }
	inline int32_t* get_address_of_maxLife_10() { return &___maxLife_10; }
	inline void set_maxLife_10(int32_t value)
	{
		___maxLife_10 = value;
	}

	inline static int32_t get_offset_of_bulletSpeed_11() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___bulletSpeed_11)); }
	inline float get_bulletSpeed_11() const { return ___bulletSpeed_11; }
	inline float* get_address_of_bulletSpeed_11() { return &___bulletSpeed_11; }
	inline void set_bulletSpeed_11(float value)
	{
		___bulletSpeed_11 = value;
	}

	inline static int32_t get_offset_of_life_12() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___life_12)); }
	inline int32_t get_life_12() const { return ___life_12; }
	inline int32_t* get_address_of_life_12() { return &___life_12; }
	inline void set_life_12(int32_t value)
	{
		___life_12 = value;
	}

	inline static int32_t get_offset_of_gameControl_13() { return static_cast<int32_t>(offsetof(Player_t2393081601, ___gameControl_13)); }
	inline GameControl_t2485871851 * get_gameControl_13() const { return ___gameControl_13; }
	inline GameControl_t2485871851 ** get_address_of_gameControl_13() { return &___gameControl_13; }
	inline void set_gameControl_13(GameControl_t2485871851 * value)
	{
		___gameControl_13 = value;
		Il2CppCodeGenWriteBarrier(&___gameControl_13, value);
	}
};

struct Player_t2393081601_StaticFields
{
public:
	// System.Collections.Generic.List`1<UnityEngine.Color> Player::bulletGauge
	List_1_t1267765161 * ___bulletGauge_2;

public:
	inline static int32_t get_offset_of_bulletGauge_2() { return static_cast<int32_t>(offsetof(Player_t2393081601_StaticFields, ___bulletGauge_2)); }
	inline List_1_t1267765161 * get_bulletGauge_2() const { return ___bulletGauge_2; }
	inline List_1_t1267765161 ** get_address_of_bulletGauge_2() { return &___bulletGauge_2; }
	inline void set_bulletGauge_2(List_1_t1267765161 * value)
	{
		___bulletGauge_2 = value;
		Il2CppCodeGenWriteBarrier(&___bulletGauge_2, value);
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
