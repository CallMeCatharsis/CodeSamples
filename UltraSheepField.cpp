/*
Ultra Sheep Field was written in C++ with a basic engine made from DirectX10

This is a small part of the source code for Ultra Sheep Field
This sample file shows the initialization of the main game states,
the de-initialization of the main games states, and the rendering
function for all parts of the game. It also contains the small ExitCheck
function used in the main game loop for checking if "Exit" has been chosen
from the main menu.

https://github.com/CallMeCatharsis
hirobadams@gmail.com
*/

#include "..\Header Files\UltraSheepField.h"

UltraSheepField* UltraSheepField::s_pUltraSheepField = NULL;

// UltraSheepField class constructor
UltraSheepField::UltraSheepField()
	{}

// UltraSheepField class destructor
UltraSheepField::~UltraSheepField()
	{
		this->Release();
	}

// Create and initialize the game state pointers with
// new game state objects and initialize important Game
// state variables
bool UltraSheepField::Init()
	{
		m_pBackGroundGameState = new BackGroundGameState();
		if (!m_pBackGroundGameState->Init())
		{
			return false;
		}

		m_pMenuState = new MenuState();
		if (!m_pMenuState->Init())
		{
			return false;
		}

		m_pPlayerOneState = new PlayerOneState();
		if (!m_pPlayerOneState->Init())
		{
			return false;
		}

		m_pPlayerTwoState = new PlayerTwoState();
		if (!m_pPlayerTwoState->Init())
		{
			return false;
		}

		// Adjust music volume
		// Set the default game state
		// and make sure the vitory condition is false
		MusicVolume = 0.5f;
		m_CurrentGameState = 0;
		PlayerOneVictory = false;

		return true;
	}

// When the game has been launched and the get function has
// been called from the main game loop, initialize the static pointer
// for the main game object with a new instance of UltraSheepField
UltraSheepField& UltraSheepField::Get()
	{
		if (s_pUltraSheepField == NULL)
		{
			s_pUltraSheepField = new UltraSheepField();
		}
		return *s_pUltraSheepField;
	}

// Delete the main game state static pointer from heap memory
// but only if they game is exiting and the game state is not equal
// to NULL
void UltraSheepField::Kill()
	{
		if (s_pUltraSheepField != NULL)
		{
			delete s_pUltraSheepField;
		}
	}

// Delete the game state pointers from heap memory
// when the game exits
void UltraSheepField::Release()
	{
		delete m_pBackGroundGameState;
		delete m_pMenuState;
		delete m_pPlayerOneState;
		delete m_pPlayerTwoState;
	}

// Main function for rendering every individual state of the game
void UltraSheepField::Render()
	{
		// clear the back buffer and the depth\stencil buffer.
		Graphics2D::Get().Clear(D3DXCOLOR(0.0f, 0.0f, 0.0f, 0.0f));
		// Start drawing game states.
		HR(Graphics2D::Get().GetSprite()->Begin(D3DX10_SPRITE_SORT_TEXTURE));

		// Switch statement for rendering specific game states
		switch (m_CurrentGameState)
		{
			case 0:
				// Render the main menu
				m_pMenuState->Render();

				// If the game music is playing when actually the menu music should be playing,
				// fade out the game music using the FadeGameMusic method and start playing the
				// Menu music!
				if(m_pBackGroundGameState->GetBackGroundMusic()->IsPlaying())
				{
					FadeGameMusic();
				}
				// Otherwise if the required music isn't already playing, then reset it and play it!
				// Doing this also makes the music loop by itself without explicitly telling it to!
				else if(!m_pBackGroundGameState->GetMenuMusic()->IsPlaying())
				{
					m_pBackGroundGameState->GetMenuMusic()->Reset();
					m_pBackGroundGameState->GetMenuMusic()->SetVolume(0.5);
					m_pBackGroundGameState->GetMenuMusic()->Play();
				}
				break;

			case 1:
				// Render the instructions screen
				m_pBackGroundGameState->RenderInstructions();
				break;

			case 2:
				// Render each game state.
				m_pBackGroundGameState->Render(); // Simply renders the game background
				m_pPlayerOneState->Render();
				m_pPlayerTwoState->Render();

				// If the menu music is playing when actually the game music should be playing,
				// fade out the menu music using the FadeMenuMusic method and start playing the
				// game music!
				if(m_pBackGroundGameState->GetMenuMusic()->IsPlaying())
				{
					FadeMenuMusic();
				}
				// Otherwise if the required music isn't already playing, then reset it and play it!
				// Doing this also makes the music loop by itself without explicitly telling it to!
				else if(!m_pBackGroundGameState->GetBackGroundMusic()->IsPlaying())
				{
					m_pBackGroundGameState->GetBackGroundMusic()->Reset();
					m_pBackGroundGameState->GetBackGroundMusic()->SetVolume(0.5);
					m_pBackGroundGameState->GetBackGroundMusic()->Play();
				}
				break;

			case 3:
				// Render the victor and the results screen!
				m_pBackGroundGameState->Render(); // Render the game background before the results

				// Render the victor behind the results, but above the background
				if (PlayerOneVictory == true)
				{
					m_pPlayerOneState->Render();
				}
				else
				{
					m_pPlayerTwoState->Render();
				}

			  m_pBackGroundGameState->RenderResults();
				break;

			default:
				m_pMenuState->Render();
		}
		// Finished rendering.
		HR(Graphics2D::Get().GetSprite()->End());
		// display the next item in the swap chain
		HR(Graphics2D::Get().GetSwapChain()->Present(1, 0));
	}

// This function allows the main game loop to check if
// exit has been chosen from the main menu game state
bool UltraSheepField::ExitCheck()
{
	if(m_pMenuState->ExitChosen())
		return true;
	else
		return false;
}
