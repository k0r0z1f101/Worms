using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Inventory
{
    private string _nameOfWeapon;
    private int _BazookaammoQty;
    private int _grenadesAmmo;
    private bool _isAmmoInfinite;
    private bool _bazookaSelected;
    private bool _holyHandGrenadeSelected;
    private bool _ninjaRopeSelected;

    public void SetAmmo(int bazookarounds, int grenades)
    {
        _BazookaammoQty = bazookarounds;
        _grenadesAmmo = grenades;
    }
    
    public int GetBazookaAmmo()
    {
        return _BazookaammoQty;
    }

    public int GetGrenades()
    {
        return _grenadesAmmo;
    }

    public void SelectInitialWeapon()
    {
        _bazookaSelected = true;
        _holyHandGrenadeSelected = false;
        _ninjaRopeSelected = false;
    }

    public void WeaponSwap()
    {
        if (_bazookaSelected)
        {
            _bazookaSelected = false;
            _holyHandGrenadeSelected = true;
            _ninjaRopeSelected = false;
        }
        else if(_holyHandGrenadeSelected)
        {
            _bazookaSelected = false;
            _holyHandGrenadeSelected = false;
            _ninjaRopeSelected = true;
        }
        else if (_ninjaRopeSelected)
        {
            _bazookaSelected = true;
            _holyHandGrenadeSelected = false;
            _ninjaRopeSelected = false;
        }
    }

    public int ReturnSelectedWeapon()
    {
        if (_bazookaSelected)
        {
            return 0;
        }
        else if (_holyHandGrenadeSelected)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
}