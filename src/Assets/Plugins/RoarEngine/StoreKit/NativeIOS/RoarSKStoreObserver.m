
#import "RoarSKStoreObserver.h"

// Based on the sample code in the official In-App Purchase Programming Guide.
// @see http://developer.apple.com/library/ios/#documentation/NetworkingInternet/Conceptual/StoreKitGuide/AddingaStoretoYourApplication/AddingaStoretoYourApplication.html

@class RoarSKStore;

@implementation RoarSKStoreObserver// : NSObject<SKPaymentTransactionObserver>

@synthesize roarSKStore;

- (RoarSKStoreObserver*)initWithRoarSKStore:(RoarSKStore *)store
{
	self = [super init];
	roarSKStore = store;
    return self;
}

// Called whenever new transactions are created or updated,
// simply sends the activity to the RoarSKStore instance.
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
            	// tell the RoarSKStore that we have a successful transaction
    			[roarSKStore onCompleteTransaction:transaction forProduct:transaction.payment.productIdentifier];
    			// remove the transaction from the payment queue.
    			[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                break;
            case SKPaymentTransactionStateFailed:
            	// inform the RoarSKStore that we have a failed transaction
            	[roarSKStore onFailedTransaction:transaction forProduct:transaction.payment.productIdentifier];
				// remove the transaction from the payment queue.
				[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
                break;
            case SKPaymentTransactionStateRestored:
            	// TODO: currently unhandled
                break;
            default:
                break;
        }
    }
}

@end
