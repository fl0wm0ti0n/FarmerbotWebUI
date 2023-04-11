My nodes
!!farmerbot.nodemanager.define
    id:13
    twinid:261

!!farmerbot.nodemanager.define
    id:17
    twinid:269

!!farmerbot.nodemanager.define     
    id:18
    twinid:270

!!farmerbot.nodemanager.define     
    id:19     
    twinid:295
    never_shutdown:true

Farm configuration
!!farmerbot.farmmanager.define
    id:158
    public_ips:0

Power configuration
!!farmerbot.powermanager.define
    periodic_wakeup:8:30AM
    wake_up_threshold:85
