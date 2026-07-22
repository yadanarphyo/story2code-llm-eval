Ì
™/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_qwen-3.5/run1/src/Services/IGetNearbyRecyclingFacilitiesService.cs
	namespace 	
Implementation
 
. 
Services !
;! "
public 
	interface 0
$IGetNearbyRecyclingFacilitiesService 5
{ 
IEnumerable 
< 
RecyclingFacility !
>! "(
GetNearbyRecyclingFacilities# ?
(? @
string@ F
zipCodeG N
)N O
;O P
} Œ
˜/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_qwen-3.5/run1/src/Services/GetNearbyRecyclingFacilitiesService.cs
	namespace 	
Implementation
 
. 
Services !
;! "
public 
class /
#GetNearbyRecyclingFacilitiesService 0
:1 20
$IGetNearbyRecyclingFacilitiesService3 W
{ 
private 
readonly 
List 
< 
RecyclingFacility +
>+ ,
_facilities- 8
;8 9
public		 
/
#GetNearbyRecyclingFacilitiesService		 .
(		. /
)		/ 0
{

 
_facilities 
= 
new 
List 
< 
RecyclingFacility 0
>0 1
{ 	
new 
RecyclingFacility !
{ 

FacilityId 
= 
$num 
, 
Name 
= 
$str +
,+ ,
Address 
= 
$str (
,( )
City 
= 
$str &
,& '
State 
= 
$str 
, 
ZipCode 
= 
$str !
,! "
DistanceInMiles 
=  !
$num" %
,% &
PhoneNumber 
= 
$str (
} 
, 
new 
RecyclingFacility !
{ 

FacilityId 
= 
$num 
, 
Name 
= 
$str +
,+ ,
Address 
= 
$str +
,+ ,
City 
= 
$str &
,& '
State 
= 
$str 
, 
ZipCode   
=   
$str   !
,  ! "
DistanceInMiles!! 
=!!  !
$num!!" %
,!!% &
PhoneNumber"" 
="" 
$str"" (
}## 
,## 
new$$ 
RecyclingFacility$$ !
{%% 

FacilityId&& 
=&& 
$num&& 
,&& 
Name'' 
='' 
$str'' 0
,''0 1
Address(( 
=(( 
$str(( (
,((( )
City)) 
=)) 
$str)) !
,))! "
State** 
=** 
$str** 
,** 
ZipCode++ 
=++ 
$str++ !
,++! "
DistanceInMiles,, 
=,,  !
$num,," %
,,,% &
PhoneNumber-- 
=-- 
$str-- (
}.. 
}// 	
;//	 

}00 
public22 

IEnumerable22 
<22 
RecyclingFacility22 (
>22( )(
GetNearbyRecyclingFacilities22* F
(22F G
string22G M
zipCode22N U
)22U V
{33 
if44 

(44 
string44 
.44 
IsNullOrWhiteSpace44 %
(44% &
zipCode44& -
)44- .
)44. /
{55 	
return66 

Enumerable66 
.66 
Empty66 #
<66# $
RecyclingFacility66$ 5
>665 6
(666 7
)667 8
;668 9
}77 	
return:: 
_facilities:: 
.:: 
Where::  
(::  !
f::! "
=>::# %
f::& '
.::' (
ZipCode::( /
.::/ 0

StartsWith::0 :
(::: ;
zipCode::; B
)::B C
)::C D
;::D E
};; 
}<< ®
s/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_qwen-3.5/run1/src/Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. 
	AddScoped 
< 0
$IGetNearbyRecyclingFacilitiesService ?
,? @/
#GetNearbyRecyclingFacilitiesServiceA d
>d e
(e f
)f g
;g h
var		 
app		 
=		 	
builder		
 
.		 
Build		 
(		 
)		 
;		 
app 
. 
MapControllers 
( 
) 
; 
app 
. 
Run 
( 
) 	
;	 
œ
„/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_qwen-3.5/run1/src/Models/RecyclingFacility.cs
	namespace 	
Implementation
 
. 
Models 
;  
public 
class 
RecyclingFacility 
{ 
public 

int 

FacilityId 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
public 

string 
City 
{ 
get 
; 
set !
;! "
}# $
=% &
string' -
.- .
Empty. 3
;3 4
public		 

string		 
State		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
=		& '
string		( .
.		. /
Empty		/ 4
;		4 5
public

 

string

 
ZipCode

 
{

 
get

 
;

  
set

! $
;

$ %
}

& '
=

( )
string

* 0
.

0 1
Empty

1 6
;

6 7
public 

double 
DistanceInMiles !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

string 
PhoneNumber 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
} â
•/Users/yadanarphyo/Space/ESS/Dissertation/Story2Code/Runs/2026-07-18_run2/UserStory-01_qwen-3.5/run1/src/Controllers/RecyclingFacilitiesController.cs
	namespace 	
Implementation
 
. 
Controllers $
;$ %
[ 
Route 
( 
$str !
)! "
]" #
[ 
ApiController 
] 
public		 
class		 )
RecyclingFacilitiesController		 *
:		+ ,
ControllerBase		- ;
{

 
private 
readonly 0
$IGetNearbyRecyclingFacilitiesService 9
_service: B
;B C
public 
)
RecyclingFacilitiesController (
(( )0
$IGetNearbyRecyclingFacilitiesService) M
serviceN U
)U V
{ 
_service 
= 
service 
; 
} 
[ 
HttpGet 
] 
public 

ActionResult 
< 
IEnumerable #
<# $
RecyclingFacility$ 5
>5 6
>6 7
Get8 ;
(; <
[< =
	FromQuery= F
]F G
stringH N
zipCodeO V
)V W
{ 
var 
result 
= 
_service 
. (
GetNearbyRecyclingFacilities :
(: ;
zipCode; B
)B C
;C D
return 
Ok 
( 
result 
) 
; 
} 
} 