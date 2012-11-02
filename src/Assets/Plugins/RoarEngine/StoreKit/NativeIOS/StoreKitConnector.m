
#import "StoreKitConnector.h"

static RoarSKStore* gRoarSKStore = nil;

void _StoreKitInit(const char* unityGameObject) {
    if (gRoarSKStore == nil) {
        gRoarSKStore = [[RoarSKStore alloc] init:unityGameObject];
    } else {
	    [gRoarSKStore setUnityListener:unityGameObject];
    }
}

bool _StoreKitPurchasesEnabled() {
	return [gRoarSKStore purchasesEnabled];
}

void _StoreKitRequestProductData(const char* cProductIdentifiers) {
    [gRoarSKStore requestProductData:cProductIdentifiers];
}

void _StoreKitPurchaseQuantity(const char* cProductIdentifier, int quantity) {
    [gRoarSKStore purchaseProduct:cProductIdentifier withQuantity:quantity];
}

void _StoreKitPurchase(const char* cProductIdentifier) {
    [gRoarSKStore purchaseProduct:cProductIdentifier];
}
